using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Infrastructure.Repositories.Helpers;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripRepository : Repository<Trip>, ITripRepository
{
    public TripRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Trip?> GetByIdForChargeUpdateAsync(long id) =>
        await _dbSet
            .Include(t => t.TripCharges)
            .FirstOrDefaultAsync(t => t.Id == id);

    public override async Task<Trip?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(t => t.Driver)
            .Include(t => t.Contractor)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.Company)
                    .ThenInclude(c => c!.CompanyDetail)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.ContractExtraFees)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.ContractDocuments
                    .Where(cd => cd.Document!.NameEn == ContractDocumentNames.ContractConditions))
                .ThenInclude(cd => cd.Document)
            .Include(t => t.ContractVehicleType)
                .ThenInclude(cvt => cvt!.VehicleType)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.ReturnDestination!)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.ReturnDestination!)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Driver)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Color)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.FuelType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Sector)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.TailVehicles)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.VehicleAssignmentSnapshot)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.Waypoints)
            .Include(t => t.TripCharges)
            .Include(t => t.Invoice)
            .Include(t => t.ClaimInvoice)
            .Include(t => t.ContractorPayout)
            .Include(t => t.Lead)
            .Include(t => t.Dispatcher)
            .Include(t => t.AuditedBy)
            .Include(t => t.Recordings!.Where(r => r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT))
            .Include(t => t.TripDocuments)
                .ThenInclude(td => td.Document)
            .Include(t => t.BookingRequests.Where(r => r.Status == TripBookingRequestStatus.Pending))
                .ThenInclude(r => r.Contractor)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<(IEnumerable<Trip> Trips, int TotalCount)> SearchAsync(
        long? contractId = null,
        long? vehicleId = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(t => !contractId.HasValue || t.ContractId == contractId.Value)
            .Where(t => !vehicleId.HasValue || t.VehicleId == vehicleId.Value)
            .Where(t => !startDateFrom.HasValue || t.StartDate >= startDateFrom.Value)
            .Where(t => !startDateTo.HasValue || t.StartDate <= startDateTo.Value)
            .OrderByDescending(t => t.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(t => !contractId.HasValue || t.ContractId == contractId.Value)
            .Where(t => !vehicleId.HasValue || t.VehicleId == vehicleId.Value)
           .Where(t => !startDateFrom.HasValue || t.StartDate >= startDateFrom.Value)
            .Where(t => !startDateTo.HasValue || t.StartDate <= startDateTo.Value)
            .CountAsync());

    public async Task<IEnumerable<Trip>> GetByContractIdAsync(long contractId, DateTime? fromDate = null, DateTime? toDate = null) =>
        await _dbSet
            .Where(t => t.ContractId == contractId)
            .Where(t => t.DeletedAt == null)
            .Where(t => !fromDate.HasValue || t.StartDate >= fromDate.Value)
            .Where(t => !toDate.HasValue || t.EndDate <= toDate.Value)
            .ToListAsync();

    public async Task<IEnumerable<Trip>> GetByVehicleIdsAsync(List<long> vehicleIds) =>
        await _dbSet
            .Where(t => t.VehicleId.HasValue && vehicleIds.Contains(t.VehicleId.Value))
            .ToListAsync();

    // Clears or sets driver on open trips; Scheduled → Pending when driver cleared.
    public async Task<int> UpdateDriverForVehicleTripsAsync(long vehicleId, long? newDriverId, DateTime minStartDateInclusive)
    {
        var statuses = new List<TripStatus> { TripStatus.Pending, TripStatus.Scheduled };
        var trips = await _dbSet
            .Where(t => t.VehicleId == vehicleId
                && statuses.Contains(t.Status)
                && t.DeletedAt == null
                && t.StartDate.HasValue
                && t.StartDate >= minStartDateInclusive)
            .ToListAsync();
        foreach (var trip in trips)
        {
            trip.DriverId = newDriverId;
            if (!newDriverId.HasValue && trip.Status == TripStatus.Scheduled)
                trip.Status = TripStatus.Pending;
            _dbSet.Update(trip);
        }
        return trips.Count;
    }

    public async Task<IReadOnlyList<Trip>> GetTripsByVehicleAndStatusesAsync(long vehicleId, IEnumerable<TripStatus> statuses, DateTime minStartDateInclusive)
    {
        var statusList = statuses.ToList();
        return await _dbSet
            .Where(t => t.VehicleId == vehicleId
                && statusList.Contains(t.Status)
                && t.DeletedAt == null
                && t.StartDate.HasValue
                && t.StartDate >= minStartDateInclusive)
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    private static string EscapeForILike(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }

    public IQueryable<Trip> QueryWithoutIncludes() => base.Query();

    public override IQueryable<Trip> Query() =>
        base.Query()
            .Include(t => t.Driver)
            .Include(t => t.Contractor)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.Company)
                    .ThenInclude(c => c!.CompanyDetail)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.ContractExtraFees)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.ContractDocuments
                    .Where(cd => cd.Document!.NameEn == ContractDocumentNames.ContractConditions))
                .ThenInclude(cd => cd.Document)
            .Include(t => t.ContractVehicleType)
                .ThenInclude(cvt => cvt!.VehicleType)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.ReturnDestination!)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(t => t.ContractDestinationAssignment!)
                .ThenInclude(tcd => tcd.ReturnDestination!)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Driver)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Color)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.FuelType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Sector)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.VehicleAssignmentSnapshot)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.Waypoints)
            .Include(t => t.TripCharges)
            .Include(t => t.Invoice)
            .Include(t => t.ClaimInvoice)
            .Include(t => t.ContractorPayout)
            .Include(t => t.Lead)
            .Include(t => t.Dispatcher)
            .Include(t => t.AuditedBy)
            .Include(t => t.Recordings!.Where(r => r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT))
            .Include(t => t.TripDocuments)
                .ThenInclude(td => td.Document)
            .Include(t => t.BookingRequests.Where(r => r.Status == TripBookingRequestStatus.Pending))
                .ThenInclude(r => r.Contractor)
            .AsSplitQuery();

    public async Task<(List<Trip> Trips, int TotalCount)> ListAsync(
        long? contractId,
        string? contractReferenceNumber,
        long? companyId,
        List<long>? matchingCompanyIds,
        string? companyName,
        string? driverName,
        string? plate,
        long? vehicleId,
        long? vehicleTypeId,
        List<long>? sectorIds,
        long? driverId,
        IEnumerable<long>? leadIds,
        bool? isExternalTrip,
        List<long>? contractorIds,
        bool? hasContractor,
        int? status,
        IEnumerable<TripDocumentStatus>? tripDocumentStatuses,
        int? overNightCostBearer,
        int? dispatchingMode,
        IEnumerable<int>? statuses,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        OperationalDriverStatus? currentDriverStatus,
        TripLifecycleActor? lifecycleActor,
        string? search,
        int? page,
        int? pageSize,
        bool? isBackfill = null,
        List<long>? destinationIds = null,
        long? invitedContractorId = null,
        bool? openInviteOnly = null,
        bool? isPushedToContractors = null,
        bool? hasContractorInvoice = null,
        bool? isContractorPaid = null,
        bool? isCreatedByCompany = null,
        long? contractVehicleTypeId = null,
        ContractType? contractTypeId = null,
        DateOnly? financialDateFrom = null,
        DateOnly? financialDateTo = null)
    {
        var contractRefPattern = string.IsNullOrWhiteSpace(contractReferenceNumber)
            ? null
            : "%" + EscapeForILike(contractReferenceNumber.Trim()) + "%";
        var companyNamePattern = string.IsNullOrWhiteSpace(companyName)
            ? null
            : "%" + EscapeForILike(companyName.Trim()) + "%";
        var driverNamePattern = string.IsNullOrWhiteSpace(driverName)
            ? null
            : "%" + EscapeForILike(driverName.Trim()) + "%";
        var platePattern = string.IsNullOrWhiteSpace(plate)
            ? null
            : "%" + EscapeForILike(plate.Trim()) + "%";
        long? tripIdSearch = search?.Trim() is { } trimmed
            && trimmed.StartsWith(TripReferenceNumber.Prefix, StringComparison.OrdinalIgnoreCase)
            && long.TryParse(trimmed.AsSpan(TripReferenceNumber.Prefix.Length), out var tripId)
            && tripId > 0
            ? tripId
            : null;

        var searchPattern = tripIdSearch.HasValue || string.IsNullOrWhiteSpace(search)
            ? null
            : "%" + EscapeForILike(search.Trim()) + "%";

        IQueryable<Trip> ApplyFilters(IQueryable<Trip> q)
        {
            q = q
                .Where(t => !contractId.HasValue || t.ContractId == contractId.Value)
                .Where(t => !contractTypeId.HasValue ||
                    (t.Contract != null && t.Contract.ContractType == contractTypeId.Value))
                .Where(t => contractRefPattern == null ||
                    (t.Contract != null && EF.Functions.ILike(t.Contract.ReferenceNumber, contractRefPattern)))
                .Where(t => !companyId.HasValue ||
                    (t.Contract != null && t.Contract.Company != null && t.Contract.Company.Id == companyId.Value))
                .Where(t => (matchingCompanyIds == null || !matchingCompanyIds.Any()) ||
                    (t.Contract != null && t.Contract.Company != null && matchingCompanyIds.Contains(t.Contract.Company.Id)))
                .Where(t => companyNamePattern == null ||
                    (t.Contract != null &&
                     t.Contract.Company != null &&
                     t.Contract.Company.CompanyDetail != null &&
                     (EF.Functions.ILike(t.Contract.Company.CompanyDetail.NameEn, companyNamePattern) ||
                      (t.Contract.Company.CompanyDetail.NameAr != null &&
                       EF.Functions.ILike(t.Contract.Company.CompanyDetail.NameAr, companyNamePattern)))))
                .Where(t => driverNamePattern == null ||
                    (t.Driver != null &&
                     (EF.Functions.ILike(t.Driver.FullName, driverNamePattern) ||
                      (t.Driver.FullNameAr != null &&
                       EF.Functions.ILike(t.Driver.FullNameAr, driverNamePattern)))))
                .Where(t => platePattern == null ||
                    (t.VehicleAssignmentSnapshot != null &&
                     (EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateNo, platePattern) ||
                      EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateLetters, platePattern) ||
                      EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateLetters + " " + t.VehicleAssignmentSnapshot.HeadPlateNo, platePattern) ||
                      EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateLetters + t.VehicleAssignmentSnapshot.HeadPlateNo, platePattern)))
                    || (t.Vehicle != null &&
                     (EF.Functions.ILike(t.Vehicle.PlateNo, platePattern) ||
                      EF.Functions.ILike(t.Vehicle.PlateLetters, platePattern) ||
                      EF.Functions.ILike(t.Vehicle.PlateLetters + " " + t.Vehicle.PlateNo, platePattern) ||
                      EF.Functions.ILike(t.Vehicle.PlateLetters + t.Vehicle.PlateNo, platePattern))))
                .Where(t => !vehicleId.HasValue || t.VehicleId == vehicleId.Value)
                .Where(t => !vehicleTypeId.HasValue ||
                    (t.ContractVehicleType != null && t.ContractVehicleType.VehicleTypeId == vehicleTypeId.Value))
                .Where(t => !contractVehicleTypeId.HasValue || t.ContractVehicleTypeId == contractVehicleTypeId.Value)
                .Where(t => !isCreatedByCompany.HasValue ||
                    (isCreatedByCompany.Value ? t.CompanyUserId != null : t.CompanyUserId == null))
                .Where(t => sectorIds == null || sectorIds.Count == 0 ||
                    (t.Vehicle != null && t.Vehicle.SectorId.HasValue && sectorIds.Contains(t.Vehicle.SectorId.Value)))
                .Where(t => destinationIds == null || destinationIds.Count == 0 ||
                    (t.ContractDestinationAssignment != null
                        && t.ContractDestinationAssignment.DeletedAt == null
                        && destinationIds.Contains(t.ContractDestinationAssignment.DestinationId)))
                .Where(t => !driverId.HasValue || t.DriverId == driverId.Value)
                .Where(t => leadIds == null || !leadIds.Any() || (t.LeadId.HasValue && leadIds.Contains(t.LeadId.Value)))
                .Where(t => !isExternalTrip.HasValue || (isExternalTrip.Value ? t.LeadId != null : t.LeadId == null))
                .Where(t => contractorIds == null || contractorIds.Count == 0 ||
                    (t.ContractorId.HasValue && contractorIds.Contains(t.ContractorId.Value)))
                .Where(t => !hasContractor.HasValue ||
                    (hasContractor.Value ? t.ContractorId != null : t.ContractorId == null))
                .Where(t => !invitedContractorId.HasValue ||
                    (t.Status > TripStatus.Pending
                        ? t.ContractorId == invitedContractorId.Value
                        : openInviteOnly == true
                            ? t.BookingInvitations.Any(i =>
                                  i.ContractorId == invitedContractorId.Value
                                  && i.Status == TripBookingInvitationStatus.Pending
                                  && i.TripBookingInvitationRound != null
                                  && i.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active)
                              && !t.BookingRequests.Any(r =>
                                  r.ContractorId == invitedContractorId.Value
                                  && r.Status == TripBookingRequestStatus.Pending
                                  && r.TripBookingInvitationRound != null
                                  && r.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active)
                            : t.ContractorId == invitedContractorId.Value
                              || t.BookingInvitations.Any(i => i.ContractorId == invitedContractorId.Value
                                  && i.Status == TripBookingInvitationStatus.Pending)))
                .Where(t => !status.HasValue || t.Status == (TripStatus)status.Value)
                .Where(t => tripDocumentStatuses == null || !tripDocumentStatuses.Any() || tripDocumentStatuses.Contains(t.TripDocumentStatus))
                .Where(t => !overNightCostBearer.HasValue ||
                    (t.OverNightCostBearer ?? OverNightCostBearer.None) == (OverNightCostBearer)overNightCostBearer.Value)
                .Where(t => !dispatchingMode.HasValue || t.DispatchingMode == (TripDispatchingMode)dispatchingMode.Value)
                .Where(t => !isPushedToContractors.HasValue ||
                    (isPushedToContractors.Value
                        ? t.BookingInvitationRounds.Any(r => r.Status == TripBookingInvitationRoundStatus.Active)
                          || t.BookingRequests.Any(r =>
                              r.Status == TripBookingRequestStatus.Pending
                              || r.Status == TripBookingRequestStatus.Approved)
                        : !t.BookingInvitationRounds.Any(r => r.Status == TripBookingInvitationRoundStatus.Active)
                          && !t.BookingRequests.Any(r =>
                              r.Status == TripBookingRequestStatus.Pending
                              || r.Status == TripBookingRequestStatus.Approved)))
                .Where(t => !hasContractorInvoice.HasValue ||
                    (hasContractorInvoice.Value
                        ? t.ContractorPayoutId != null
                        : t.ContractorPayoutId == null))
                .Where(t => !isContractorPaid.HasValue ||
                    (isContractorPaid.Value
                        ? t.ContractorPayoutId != null
                          && t.ContractorPayout != null
                          && t.ContractorPayout.Status == ContractorPayoutStatus.Paid
                        : t.ContractorPayoutId == null
                          || t.ContractorPayout == null
                          || t.ContractorPayout.Status != ContractorPayoutStatus.Paid))
                .Where(t => statuses == null || !statuses.Any() || statuses.Contains((int)t.Status))
                .Where(t => !startDateFrom.HasValue || t.StartDate >= startDateFrom.Value)
                .Where(t => !startDateTo.HasValue || t.StartDate <= startDateTo.Value)
                .Where(t => !financialDateFrom.HasValue ||
                    (t.FinancialDate != null && t.FinancialDate >= financialDateFrom.Value))
                .Where(t => !financialDateTo.HasValue ||
                    (t.FinancialDate != null && t.FinancialDate <= financialDateTo.Value))
                .Where(t => !isBackfill.HasValue || t.IsBackfill == isBackfill.Value);

            if (currentDriverStatus.HasValue)
                q = OperationalDriverStatusQueryFilter.Apply(q, currentDriverStatus.Value);

            if (lifecycleActor.HasValue)
            {
                q = lifecycleActor.Value switch
                {
                    TripLifecycleActor.Driver => q.Where(t =>
                        (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                        && t.Recordings!.Any(r =>
                            (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                            && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED)
                        && !t.Recordings!.Any(r =>
                            (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                            && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                            && r.AddedByModelType != "Driver")),
                    TripLifecycleActor.Dispatcher => q.Where(t =>
                        (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                        && (!t.Recordings!.Any(r =>
                                (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                                && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED)
                            || !t.Recordings!.Any(r =>
                                (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                                && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                                && r.AddedByModelType != "Admin"))),
                    TripLifecycleActor.Assisted => q.Where(t =>
                        (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                        && t.Recordings!.Any(r =>
                            (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                            && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                            && r.AddedByModelType == "Driver")
                        && t.Recordings!.Any(r =>
                            (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                            && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                            && r.AddedByModelType == "Admin")),
                    _ => q
                };
            }

            q = q
                .Where(t =>
                    (!tripIdSearch.HasValue && searchPattern == null)
                    || (tripIdSearch.HasValue && t.Id == tripIdSearch.Value)
                    || (searchPattern != null && (
                        EF.Functions.ILike(t.Id.ToString(), searchPattern) ||
                        (t.Contract != null && (
                            EF.Functions.ILike(t.Contract.ReferenceNumber, searchPattern) ||
                            EF.Functions.ILike(t.Contract.NameEn, searchPattern) ||
                            EF.Functions.ILike(t.Contract.NameAr, searchPattern)
                        )) ||
                        (t.VehicleAssignmentSnapshot != null && (
                            EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateNo, searchPattern) ||
                            EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateLetters, searchPattern) ||
                            EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateNo + " " + t.VehicleAssignmentSnapshot.HeadPlateLetters, searchPattern)
                        )) ||
                        (t.Vehicle != null && (
                            EF.Functions.ILike(t.Vehicle.PlateNo, searchPattern) ||
                            EF.Functions.ILike(t.Vehicle.PlateLetters, searchPattern) ||
                            EF.Functions.ILike(t.Vehicle.PlateNo + " " + t.Vehicle.PlateLetters, searchPattern)
                        )) ||
                        (t.Driver != null && (
                            EF.Functions.ILike(t.Driver.FullName, searchPattern) ||
                            EF.Functions.ILike(t.Driver.FullNameAr, searchPattern) ||
                            EF.Functions.ILike(t.Driver.Phone, searchPattern)
                        )))));
            return q;
        }

        IQueryable<Trip> filteredQuery = ApplyFilters(Query().AsNoTracking()).OrderByDescending(t => t.CreatedAt);

        if (page is null || pageSize is null)
        {
            List<long> tripIds = await ApplyFilters(base.Query().AsNoTracking())
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => t.Id)
                .ToListAsync();

            if (tripIds.Count == 0)
                return ([], 0);

            var tripsById = await Query()
                .AsNoTracking()
                .Where(t => tripIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id);

            var allTrips = tripIds
                .Where(tripsById.ContainsKey)
                .Select(id => tripsById[id])
                .ToList();

            return (allTrips, 0);
        }

        int totalCount = await ApplyFilters(base.Query()).CountAsync();
        var trips = await filteredQuery
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        return (trips, totalCount);
    }

    public async Task<Trip> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("trips.not_found_with_id", id);

    public async Task<IReadOnlyList<Trip>> GetByIdsAsync(IReadOnlyList<long> ids) =>
        ids.Count == 0
            ? Array.Empty<Trip>()
            : await _dbSet
                .Include(t => t.Contract)
                .Include(t => t.TripCharges)
                .Where(t => ids.Contains(t.Id))
                .OrderBy(t => t.StartDate)
                .ToListAsync();

    public async Task<IReadOnlyDictionary<long, DateTime?>> GetStartDatesByIdsAsync(IReadOnlyList<long> ids) =>
        ids.Count == 0
            ? new Dictionary<long, DateTime?>()
            : await QueryWithoutIncludes()
                .Where(trip => ids.Contains(trip.Id))
                .Select(trip => new { trip.Id, trip.StartDate })
                .ToDictionaryAsync(trip => trip.Id, trip => (DateTime?)trip.StartDate);

    public async Task<IReadOnlyDictionary<long, (DateTime? StartDate, bool HasOvernight, decimal OvernightAmount, decimal OvernightCompanyShare, bool HasFuel, decimal FuelAmount, decimal FuelDistanceKm)>> GetCalendarBillingFactsByIdsAsync(
        IReadOnlyList<long> ids)
    {
        if (ids.Count == 0)
            return new Dictionary<long, (DateTime? StartDate, bool HasOvernight, decimal OvernightAmount, decimal OvernightCompanyShare, bool HasFuel, decimal FuelAmount, decimal FuelDistanceKm)>();

        var rows = await QueryWithoutIncludes()
            .Where(trip => ids.Contains(trip.Id))
            .Select(trip => new
            {
                trip.Id,
                trip.StartDate,
                trip.OverNightCostBearer,
                trip.ContractDistanceKm,
                Charges = trip.TripCharges!
                    .Select(charge => new { charge.ChargeType, charge.Amount, charge.CompanyShare })
                    .ToList()
            })
            .ToListAsync();

        return rows.ToDictionary(row => row.Id, row =>
        {
            bool hasOvernight = row.OverNightCostBearer is OverNightCostBearer.Customer or OverNightCostBearer.Loadly;
            var fuelAmounts = row.Charges
                .Where(charge => charge.ChargeType == TripChargeType.Fuel)
                .Select(charge => charge.Amount)
                .ToList();
            bool hasFuel = fuelAmounts.Count > 0;
            decimal overnightAmount = 0m;
            decimal overnightCompanyShare = 0m;
            if (hasOvernight)
            {
                foreach (var charge in row.Charges)
                {
                    if (charge.ChargeType != TripChargeType.OverNight)
                        continue;

                    overnightAmount += charge.Amount;
                    overnightCompanyShare += charge.CompanyShare ?? 0m;
                }
            }

            return (
                row.StartDate,
                hasOvernight,
                overnightAmount,
                overnightCompanyShare,
                hasFuel,
                hasFuel ? fuelAmounts.Sum() : 0m,
                hasFuel ? row.ContractDistanceKm ?? 0m : 0m);
        });
    }

    public async Task<IReadOnlyDictionary<long, (decimal Amount, decimal CompanyShare)>> GetOvernightChargesByIdsAsync(
        IReadOnlyList<long> ids)
    {
        if (ids.Count == 0)
            return new Dictionary<long, (decimal Amount, decimal CompanyShare)>();

        var rows = await QueryWithoutIncludes()
            .Where(trip => ids.Contains(trip.Id)
                && (trip.OverNightCostBearer == OverNightCostBearer.Customer
                    || trip.OverNightCostBearer == OverNightCostBearer.Loadly))
            .Select(trip => new
            {
                trip.Id,
                Charges = trip.TripCharges!
                    .Where(charge => charge.ChargeType == TripChargeType.OverNight)
                    .Select(charge => new { charge.Amount, charge.CompanyShare })
                    .ToList()
            })
            .ToListAsync();

        return rows.ToDictionary(
            row => row.Id,
            row => (
                row.Charges.Sum(charge => charge.Amount),
                row.Charges.Sum(charge => charge.CompanyShare ?? 0m)));
    }

    public async Task<IReadOnlyDictionary<long, (decimal Amount, decimal DistanceKm)>> GetFuelChargesByIdsAsync(
        IReadOnlyList<long> ids)
    {
        if (ids.Count == 0)
            return new Dictionary<long, (decimal Amount, decimal DistanceKm)>();

        var rows = await QueryWithoutIncludes()
            .Where(trip => ids.Contains(trip.Id))
            .Select(trip => new
            {
                trip.Id,
                trip.ContractDistanceKm,
                Charges = trip.TripCharges!
                    .Where(charge => charge.ChargeType == TripChargeType.Fuel)
                    .Select(charge => charge.Amount)
                    .ToList()
            })
            .ToListAsync();

        return rows
            .Where(row => row.Charges.Count > 0)
            .ToDictionary(
                row => row.Id,
                row => (row.Charges.Sum(), row.ContractDistanceKm ?? 0m));
    }

    public async Task<int> BulkUpdateDocumentStatusAsync(IReadOnlyList<long> tripIds, long collectorId) =>
        await _dbSet
            .Where(t => tripIds.Contains(t.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.CollectorId, collectorId)
                .SetProperty(t => t.TripDocumentStatus, TripDocumentStatus.WithFactoryCollector)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow));

    public async Task<int> BulkUpdateDocumentStatusToFinanceTeamAsync(IReadOnlyList<long> tripIds, long financeCollectorId) =>
        await _dbSet
            .Where(t => tripIds.Contains(t.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.FinanceCollectorId, financeCollectorId)
                .SetProperty(t => t.TripDocumentStatus, TripDocumentStatus.WithFinanceTeam)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow));

    public async Task<IReadOnlyList<Trip>> ListActiveTripsWithLocationAsync(
        long? companyId,
        string? contractReferenceNumber,
        long? driverId,
        long? vehicleId)
    {
        var contractRefPattern = string.IsNullOrWhiteSpace(contractReferenceNumber)
            ? null
            : "%" + EscapeForILike(contractReferenceNumber.Trim()) + "%";

        return await TrackingQuery()
            .Where(t => t.Status == TripStatus.InProgress)
            .Where(t => !companyId.HasValue ||
                (t.Contract != null && t.Contract.Company != null && t.Contract.Company.Id == companyId.Value))
            .Where(t => contractRefPattern == null ||
                (t.Contract != null && EF.Functions.ILike(t.Contract.ReferenceNumber, contractRefPattern)))
            .Where(t => !driverId.HasValue || t.DriverId == driverId.Value)
            .Where(t => !vehicleId.HasValue || t.VehicleId == vehicleId.Value)
            .OrderByDescending(t => t.StartDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Trip>> GetForContractSlotBillingAsync(
        long contractSlotId,
        long vehicleId,
        long contractId,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        var tripsBySlotId = await GetForContractSlotBillingBySlotsAsync(
            contractId,
            [new ContractSlotBillingLoadCriteria(contractSlotId, vehicleId, assignedAtUtc, archivedAtUtc, assignedAtUtc)],
            periodStart,
            periodEnd);

        return tripsBySlotId[contractSlotId];
    }

    public async Task<IReadOnlyDictionary<long, IReadOnlyList<Trip>>> GetForContractSlotBillingBySlotsAsync(
        long contractId,
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        if (slots.Count == 0)
            return new Dictionary<long, IReadOnlyList<Trip>>();

        var matchingTrips = await _dbSet
            .AsNoTracking()
            .Where(t => t.DeletedAt == null)
            .Where(t => t.ContractId == contractId)
            .Where(t => t.FinancialDate >= periodStart && t.FinancialDate <= periodEnd)
            .Where(ContractSlotBillingQueryExpressions.BuildTripMatchesAnySlotExpression(slots))
            .ToListAsync();

        return slots.ToDictionary(
            slot => slot.SlotId,
            slot => (IReadOnlyList<Trip>)FilterTripsForSlot(matchingTrips, slot));
    }

    private static List<Trip> FilterTripsForSlot(IReadOnlyList<Trip> trips, ContractSlotBillingLoadCriteria slot) =>
        trips.Where(t => ContractSlotBillingTripMatcher.MatchesSlot(t, slot)).ToList();

    public async Task<bool> HasCompletedTripsForContractSlotAsync(long contractSlotId) =>
        await _dbSet
            .AsNoTracking()
            .Where(t => t.DeletedAt == null)
            .Where(t => t.ContractSlotId == contractSlotId)
            .Where(t => t.Status == TripStatus.Completed)
            .AnyAsync();

    public async Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsAsync(
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        var rows = await ApplyStartDateRange(Query(), startDateFrom, startDateTo)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task<IReadOnlyList<(long SectorId, TripStatus Status, int Count)>> GetTripStatusCountsBySectorAsync(
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        var rows = await ApplyStartDateRange(base.Query(), startDateFrom, startDateTo)
            .Where(t => t.Vehicle != null && t.Vehicle.SectorId != null)
            .GroupBy(t => new { SectorId = t.Vehicle!.SectorId!.Value, t.Status })
            .Select(g => new { g.Key.SectorId, g.Key.Status, Count = g.Count() })
            .ToListAsync();

        return rows.Select(r => (r.SectorId, r.Status, r.Count)).ToList();
    }

    public async Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsForCompanyAsync(
        long companyId,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        IQueryable<Trip> query = base.Query().Where(t => t.Contract!.CompanyId == companyId);
        var rows = await ApplyStartDateRange(query, startDateFrom, startDateTo)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.Status, x => x.Count);
    }

    private static IQueryable<Trip> ApplyStartDateRange(
        IQueryable<Trip> query,
        DateTime? startDateFrom,
        DateTime? startDateTo) =>
        query
            .Where(t => !startDateFrom.HasValue || t.StartDate >= startDateFrom.Value)
            .Where(t => !startDateTo.HasValue || t.StartDate < startDateTo.Value);

    public async Task<IReadOnlyList<(int Month, int TripsCount, decimal TotalCost)>> GetMonthlyCompletedTripAggregatesAsync(int year, int? month = null)
    {
        var rows = await Query()
            .Where(t =>
                t.StartDate >= new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                && t.StartDate < new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Where(t => t.Status == TripStatus.Completed)
            .Where(t => !month.HasValue || t.StartDate!.Value.Month == month.Value)
            .GroupBy(t => t.StartDate!.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                TripsCount = g.Count(),
                TotalCost = g.Sum(t => t.TotalTripCost ?? 0m)
            })
            .ToListAsync();

        return rows.Select(r => (r.Month, r.TripsCount, r.TotalCost)).ToList();
    }

    public async Task<IReadOnlyList<(int Year, int Month, int TripsCount, decimal TotalCost)>> GetMonthlyCompletedTripAggregatesForYearsAsync(
        IEnumerable<int> years)
    {
        var yearList = years.Distinct().ToList();
        if (yearList.Count == 0)
        {
            return Array.Empty<(int, int, int, decimal)>();
        }

        var rows = await Query()
            .Where(t => yearList.Contains(t.StartDate!.Value.Year))
            .Where(t => t.Status == TripStatus.Completed)
            .GroupBy(t => new { Year = t.StartDate!.Value.Year, Month = t.StartDate!.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TripsCount = g.Count(),
                TotalCost = g.Sum(t => t.TotalTripCost ?? 0m)
            })
            .ToListAsync();

        return rows.Select(r => (r.Year, r.Month, r.TripsCount, r.TotalCost)).ToList();
    }

    public async Task<IReadOnlyList<(int Month, int TripsCount, decimal TotalKilometers)>> GetMonthlyCompletedTripKilometerAggregatesForCompanyAsync(
        long companyId,
        int year,
        int? month = null)
    {
        var rows = await base.Query()
            .Where(t => t.Contract!.CompanyId == companyId)
            .Where(t =>
                t.StartDate >= new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                && t.StartDate < new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Where(t => t.Status == TripStatus.Completed)
            .Where(t => !month.HasValue || t.StartDate!.Value.Month == month.Value)
            .GroupBy(t => t.StartDate!.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                TripsCount = g.Count(),
                TotalKilometers = g.Sum(t => t.ContractDistanceKm ?? 0m)
            })
            .ToListAsync();

        return rows.Select(r => (r.Month, r.TripsCount, r.TotalKilometers)).ToList();
    }

    public async Task<(decimal LastSevenDays, decimal LastThirtyDays, decimal LastThreeMonths)> GetCompanyCompletedTripCostTotalsForRollingPeriodsAsync(
        long companyId,
        DateTime utcNow)
    {
        DateTime sevenDaysAgo = utcNow.AddDays(-7);
        DateTime thirtyDaysAgo = utcNow.AddDays(-30);
        DateTime threeMonthsAgo = utcNow.AddMonths(-3);

        // Aggregate the rolling windows in SQL (conditional SUMs) instead of pulling
        // three months of rows into memory and summing client-side.
        var totals = await base.Query()
            .Where(t => t.Contract!.CompanyId == companyId)
            .Where(t => t.Status == TripStatus.Completed)
            .Where(t => t.StartDate >= threeMonthsAgo && t.StartDate <= utcNow)
            .GroupBy(t => 1)
            .Select(g => new
            {
                LastSevenDays = g.Sum(t => t.StartDate >= sevenDaysAgo ? (t.TotalTripCost ?? 0m) : 0m),
                LastThirtyDays = g.Sum(t => t.StartDate >= thirtyDaysAgo ? (t.TotalTripCost ?? 0m) : 0m),
                LastThreeMonths = g.Sum(t => t.TotalTripCost ?? 0m)
            })
            .FirstOrDefaultAsync();

        return totals is null
            ? (0m, 0m, 0m)
            : (totals.LastSevenDays, totals.LastThirtyDays, totals.LastThreeMonths);
    }

    public async Task<int> GetAssignedVehicleTripsCountInDateRangeAsync(DateTime from, DateTime to) =>
        await Query()
            .Where(t => t.VehicleId != null && t.StartDate >= from && t.StartDate < to)
            .CountAsync();

    public async Task<int> GetAssignedDriverTripsCountInDateRangeAsync(DateTime from, DateTime to) =>
        await Query()
            .Where(t => t.DriverId != null && t.StartDate >= from && t.StartDate < to)
            .CountAsync();

    public async Task<decimal> GetAverageTripDurationHoursAsync()
    {
        var averageSeconds = await base.Query()
            .Where(t => (t.Status == TripStatus.Completed)
                && t.StartDate != null
                && t.EndDate != null)
            .Select(t => (double?)(t.EndDate!.Value - t.StartDate!.Value).TotalSeconds)
            .AverageAsync();

        return (decimal)((averageSeconds ?? 0) / 3600.0);
    }

    public async Task<int> GetActiveContractorTripsCountAsync() =>
        await base.Query()
            .Where(t => t.ContractorId != null && t.Contractor != null && t.Contractor.ContractorStatus == ContractorStatus.ACTIVE)
            .CountAsync();

    public async Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsForContractorAsync(long contractorId)
    {
        var rows = await Query()
            .Where(t => t.ContractorId == contractorId)
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task<(int CompletedPaidTrips, decimal CompletedPaidTripsTotalAmount, int CompletedUnpaidTrips, decimal CompletedUnpaidTripsTotalAmount)>
        GetContractorCompletedTripPaymentTotalsAsync(long contractorId)
    {
        var totals = await Query()
            .Where(t => t.ContractorId == contractorId && t.Status == TripStatus.Completed)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                PaidCount = g.Count(t =>
                    t.ContractorPayoutId != null
                    && t.ContractorPayout != null
                    && t.ContractorPayout.Status == ContractorPayoutStatus.Paid),
                PaidAmount = g.Sum(t =>
                    t.ContractorPayoutId != null
                    && t.ContractorPayout != null
                    && t.ContractorPayout.Status == ContractorPayoutStatus.Paid
                        ? (t.ContractorPrice ?? 0m)
                        : 0m),
                UnpaidCount = g.Count(t =>
                    t.ContractorPayoutId == null
                    || t.ContractorPayout == null
                    || t.ContractorPayout.Status != ContractorPayoutStatus.Paid),
                UnpaidAmount = g.Sum(t =>
                    t.ContractorPayoutId == null
                    || t.ContractorPayout == null
                    || t.ContractorPayout.Status != ContractorPayoutStatus.Paid
                        ? (t.ContractorPrice ?? 0m)
                        : 0m)
            })
            .FirstOrDefaultAsync();

        return totals is null
            ? (0, 0m, 0, 0m)
            : (totals.PaidCount, totals.PaidAmount, totals.UnpaidCount, totals.UnpaidAmount);
    }

    public async Task<(int OpenInviteTrips, int PendingBookingRequestTrips)> GetContractorInviteTripCountsAsync(long contractorId)
    {
        var openInviteTrips = await Query()
            .Where(t => t.Status == TripStatus.Pending)
            .Where(t => t.BookingInvitations.Any(i =>
                i.ContractorId == contractorId
                && i.Status == TripBookingInvitationStatus.Pending
                && i.TripBookingInvitationRound != null
                && i.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active))
            .Where(t => !t.BookingRequests.Any(r =>
                r.ContractorId == contractorId
                && r.Status == TripBookingRequestStatus.Pending
                && r.TripBookingInvitationRound != null
                && r.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active))
            .CountAsync();

        var pendingBookingRequestTrips = await Query()
            .Where(t => t.BookingRequests.Any(r =>
                r.ContractorId == contractorId
                && r.Status == TripBookingRequestStatus.Pending
                && r.TripBookingInvitationRound != null
                && r.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active))
            .CountAsync();

        return (openInviteTrips, pendingBookingRequestTrips);
    }

    public async Task<int> GetTripCountInDateRangeAsync(DateTime fromInclusive, DateTime toExclusive) =>
        await Query()
            .Where(t => t.StartDate >= fromInclusive && t.StartDate < toExclusive)
            .CountAsync();

    public async Task<IReadOnlyList<(DateTime Date, int Count)>> GetDailyActiveContractorTripCountsAsync(
        DateTime fromInclusive,
        DateTime toExclusive)
    {
        var rows = await base.Query()
            .Where(t =>
                t.ContractorId != null
                && t.Contractor != null
                && t.Contractor.ContractorStatus == ContractorStatus.ACTIVE
                && t.IsExternalTrip
                && t.StartDate >= fromInclusive
                && t.StartDate < toExclusive)
            .GroupBy(t => t.StartDate!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.Select(r => (r.Date, r.Count)).ToList();
    }

    public async Task<int> GetCompanyTripCountInDateRangeAsync(long companyId, DateTime fromInclusive, DateTime toExclusive) =>
        await base.Query()
            .Where(t => t.Contract!.CompanyId == companyId && t.StartDate >= fromInclusive && t.StartDate < toExclusive)
            .CountAsync();

    public async Task<(
        int AllTimeTrips,
        decimal AllTimeMargin,
        int CurrentMonthTrips,
        decimal CurrentMonthMargin,
        int PreviousMonthTrips,
        decimal PreviousMonthMargin)> GetContractorDashboardMetricsAsync(
        long? contractorId,
        DateTime currentMonthStartUtc)
    {
        var previousMonthStart = currentMonthStartUtc.AddMonths(-1);
        var currentMonthEnd = currentMonthStartUtc.AddMonths(1);

        var row = await Query()
            .Where(t => t.ContractorId != null && (!contractorId.HasValue || t.ContractorId == contractorId))
            .GroupBy(_ => 1)
            .Select(g => new
            {
                AllTimeTrips = g.Count(),
                AllTimeMargin = g.Sum(t =>
                    t.Status == TripStatus.Completed
                    && t.TotalTripCost != null
                    && t.ContractorPrice != null
                    && t.ContractorPrice > 0
                        ? (t.TotalTripCost ?? 0m) - (t.ContractorPrice ?? 0m)
                        : 0m),
                CurrentMonthTrips = g.Count(t => t.StartDate >= currentMonthStartUtc && t.StartDate < currentMonthEnd),
                CurrentMonthMargin = g.Sum(t =>
                    t.StartDate >= currentMonthStartUtc
                    && t.StartDate < currentMonthEnd
                    && t.Status == TripStatus.Completed
                    && t.TotalTripCost != null
                    && t.ContractorPrice != null
                    && t.ContractorPrice > 0
                        ? (t.TotalTripCost ?? 0m) - (t.ContractorPrice ?? 0m)
                        : 0m),
                PreviousMonthTrips = g.Count(t => t.StartDate >= previousMonthStart && t.StartDate < currentMonthStartUtc),
                PreviousMonthMargin = g.Sum(t =>
                    t.StartDate >= previousMonthStart
                    && t.StartDate < currentMonthStartUtc
                    && t.Status == TripStatus.Completed
                    && t.TotalTripCost != null
                    && t.ContractorPrice != null
                    && t.ContractorPrice > 0
                        ? (t.TotalTripCost ?? 0m) - (t.ContractorPrice ?? 0m)
                        : 0m)
            })
            .FirstOrDefaultAsync();

        return row is null
            ? (0, 0m, 0, 0m, 0, 0m)
            : (
                row.AllTimeTrips,
                row.AllTimeMargin,
                row.CurrentMonthTrips,
                row.CurrentMonthMargin,
                row.PreviousMonthTrips,
                row.PreviousMonthMargin);
    }

    public async Task<decimal> GetCompanyCompletedTripCostTotalAsync(
        long companyId,
        DateTime? fromInclusive = null,
        DateTime? toExclusive = null) =>
        await CompanyCompletedTripsQuery(companyId, fromInclusive, toExclusive)
            .SumAsync(t => t.TotalTripCost ?? 0m);

    public async Task<decimal> GetCompanyCompletedTripKilometersTotalAsync(
        long companyId,
        DateTime? fromInclusive = null,
        DateTime? toExclusive = null) =>
        await CompanyCompletedTripsQuery(companyId, fromInclusive, toExclusive)
            .SumAsync(t => t.ActualDistanceKm ?? 0m);

    public async Task<IReadOnlyList<(long VehicleTypeId, string NameEn, string NameAr, decimal Amount)>> GetCompanyCompletedTripCostByVehicleTypeAsync(
        long companyId,
        DateTime? fromInclusive = null,
        DateTime? toExclusive = null)
    {
        var rows = await CompanyCompletedTripsQuery(companyId, fromInclusive, toExclusive)
            .Where(t => t.Vehicle != null && t.Vehicle.VehicleType != null)
            .GroupBy(t => new
            {
                VehicleTypeId = t.Vehicle!.VehicleType!.Id,
                t.Vehicle.VehicleType.NameEn,
                t.Vehicle.VehicleType.NameAr
            })
            .Select(g => new
            {
                g.Key.VehicleTypeId,
                g.Key.NameEn,
                g.Key.NameAr,
                Amount = g.Sum(t => t.TotalTripCost ?? 0m)
            })
            .ToListAsync();

        return rows.Select(r => (r.VehicleTypeId, r.NameEn, r.NameAr, r.Amount)).ToList();
    }

    public async Task<(IReadOnlyList<(long Id, string NameEn, string NameAr, string Phone, int Driver, int Dispatcher, int Assisted, int Total)> Rows, int TotalCount)> GetDriverCompletedTripsByLifecycleActorAsync(
        long? driverId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 10)
    {
        RecordableEventType lifecycleMax = RecordableEventType.WAYPOINT_UNLOADING_COMPLETED;

        var query = Query()
            .AsNoTracking()
            .Where(t => t.Status == TripStatus.Completed)
            .Where(t => t.DriverId != null && t.Driver != null)
            .Where(t => !driverId.HasValue || t.DriverId == driverId.Value)
            .Where(t => !fromDate.HasValue || t.StartDate >= fromDate.Value)
            .Where(t => !toDate.HasValue || t.StartDate <= toDate.Value)
            .GroupBy(t => new
            {
                Id = t.DriverId!.Value,
                NameEn = t.Driver!.FullName,
                NameAr = t.Driver.FullNameAr,
                t.Driver.Phone
            })
            .Select(g => new
            {
                g.Key.Id,
                g.Key.NameEn,
                g.Key.NameAr,
                g.Key.Phone,
                Driver = g.Count(t =>
                    t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax)
                    && !t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax
                        && r.AddedByModelType != "Driver")),
                Dispatcher = g.Count(t =>
                    !t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax)
                    || !t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax
                        && r.AddedByModelType != "Admin")),
                Assisted = g.Count(t =>
                    t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax
                        && r.AddedByModelType == "Driver")
                    && t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= lifecycleMax
                        && r.AddedByModelType == "Admin")),
                Total = g.Count()
            });

        var totalCount = await query.CountAsync();
        var rows = await query
            .OrderByDescending(r => r.Total)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (
            rows.Select(r => (r.Id, r.NameEn, r.NameAr, r.Phone, r.Driver, r.Dispatcher, r.Assisted, r.Total)).ToList(),
            totalCount);
    }

    private IQueryable<Trip> CompanyCompletedTripsQuery(long companyId, DateTime? fromInclusive, DateTime? toExclusive) =>
        base.Query()
            .Where(t => t.Contract!.CompanyId == companyId)
            .Where(t => t.Status == TripStatus.Completed)
            .Where(t => !fromInclusive.HasValue || t.StartDate >= fromInclusive.Value)
            .Where(t => !toExclusive.HasValue || t.StartDate < toExclusive.Value);

    /// <summary>
    /// Query with all includes needed for tracking (single round trip).
    /// </summary>
    private IQueryable<Trip> TrackingQuery() =>
        base.Query()
            .Include(t => t.Driver)
                .ThenInclude(d => d!.DriverLocation)
            .Include(t => t.Contract)
                .ThenInclude(c => c!.Company)
                    .ThenInclude(c => c!.CompanyDetail)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Driver)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Color)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.FuelType)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.Sector)
            .Include(t => t.Vehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.VehicleAssignmentSnapshot)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.VehicleBrandModel)
                    .ThenInclude(m => m!.VehicleBrand)
            .Include(t => t.TailVehicle)
                .ThenInclude(v => v!.PlateAliases)
            .Include(t => t.Waypoints)
            .AsSingleQuery();

    public async IAsyncEnumerable<IReadOnlyList<TripExportRow>> EnumerateForExportAsync(
        TripExportCriteria criteria,
        IReadOnlyCollection<string> columns,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int batchSize = 4000;
        IQueryable<Trip> filtered = TripExportQuery.Apply(base.Query().AsNoTracking(), criteria);
        (DateTime CreatedAt, long Id)? last = null;

        while (true)
        {
            IQueryable<Trip> page = last is { } previous
                ? filtered.Where(t => t.CreatedAt < previous.CreatedAt || (t.CreatedAt == previous.CreatedAt && t.Id < previous.Id))
                : filtered;

            List<TripExportRow> trips = await TripColumnIncludes.Apply(
                    page.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id).Take(batchSize),
                    columns)
                .ToListAsync(cancellationToken);

            if (trips.Count > 0)
                yield return trips;

            if (trips.Count < batchSize)
                yield break;

            last = (trips[^1].CreatedAt, trips[^1].Id);
        }
    }
}
