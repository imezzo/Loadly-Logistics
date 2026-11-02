using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Infrastructure.Repositories.Implementations;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Infrastructure.Repositories.Helpers;

internal static class TripExportQuery
{
    public static IQueryable<Trip> Apply(IQueryable<Trip> query, TripExportCriteria criteria)
    {
        string? contractRefPattern = LikePattern(criteria.ContractReferenceNumber);
        string? search = criteria.Search;
        long? tripIdSearch = search?.Trim() is { } trimmed
            && trimmed.StartsWith(TripReferenceNumber.Prefix, StringComparison.OrdinalIgnoreCase)
            && long.TryParse(trimmed.AsSpan(TripReferenceNumber.Prefix.Length), out long tripId)
            && tripId > 0
            ? tripId
            : null;
        string? searchPattern = tripIdSearch.HasValue ? null : LikePattern(search);

        long? contractId = criteria.ContractId;
        ContractType? contractTypeId = criteria.ContractTypeId;
        long? companyId = criteria.CompanyId;
        List<long>? matchingCompanyIds = criteria.MatchingCompanyIds;
        long? vehicleId = criteria.VehicleId;
        long? vehicleTypeId = criteria.VehicleTypeId;
        List<long>? sectorIds = criteria.SectorIds;
        List<long>? destinationIds = criteria.DestinationIds;
        long? driverId = criteria.DriverId;
        IEnumerable<long>? leadIds = criteria.LeadIds;
        bool? isExternalTrip = criteria.IsExternalTrip;
        List<long>? contractorIds = criteria.ContractorIds;
        bool? hasContractor = criteria.HasContractor;
        int? status = criteria.Status;
        IEnumerable<TripDocumentStatus>? tripDocumentStatuses = criteria.TripDocumentStatuses;
        int? overNightCostBearer = criteria.OverNightCostBearer;
        int? dispatchingMode = criteria.DispatchingMode;
        IEnumerable<int>? statuses = criteria.Statuses;
        DateTime? startDateFrom = criteria.StartDateFrom;
        DateTime? startDateTo = criteria.StartDateTo;
        DateOnly? financialDateFrom = criteria.FinancialDateFrom;
        DateOnly? financialDateTo = criteria.FinancialDateTo;
        bool? isBackfill = criteria.IsBackfill;

        query = query
            .Where(t => !contractId.HasValue || t.ContractId == contractId.Value)
            .Where(t => !contractTypeId.HasValue ||
                (t.Contract != null && t.Contract.ContractType == contractTypeId.Value))
            .Where(t => contractRefPattern == null ||
                (t.Contract != null && EF.Functions.ILike(t.Contract.ReferenceNumber, contractRefPattern)))
            .Where(t => !companyId.HasValue ||
                (t.Contract != null && t.Contract.Company != null && t.Contract.Company.Id == companyId.Value))
            .Where(t => matchingCompanyIds == null || !matchingCompanyIds.Any() ||
                (t.Contract != null && t.Contract.Company != null && matchingCompanyIds.Contains(t.Contract.Company.Id)))
            .Where(t => !vehicleId.HasValue || t.VehicleId == vehicleId.Value)
            .Where(t => !vehicleTypeId.HasValue ||
                (t.ContractVehicleType != null && t.ContractVehicleType.VehicleTypeId == vehicleTypeId.Value))
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
            .Where(t => !status.HasValue || t.Status == (TripStatus)status.Value)
            .Where(t => tripDocumentStatuses == null || !tripDocumentStatuses.Any() || tripDocumentStatuses.Contains(t.TripDocumentStatus))
            .Where(t => !overNightCostBearer.HasValue ||
                (t.OverNightCostBearer ?? OverNightCostBearer.None) == (OverNightCostBearer)overNightCostBearer.Value)
            .Where(t => !dispatchingMode.HasValue || t.DispatchingMode == (TripDispatchingMode)dispatchingMode.Value)
            .Where(t => statuses == null || !statuses.Any() || statuses.Contains((int)t.Status))
            .Where(t => !startDateFrom.HasValue || t.StartDate >= startDateFrom.Value)
            .Where(t => !startDateTo.HasValue || t.StartDate <= startDateTo.Value)
            .Where(t => !financialDateFrom.HasValue || t.FinancialDate >= financialDateFrom.Value)
            .Where(t => !financialDateTo.HasValue || t.FinancialDate <= financialDateTo.Value)
            .Where(t => !isBackfill.HasValue || t.IsBackfill == isBackfill.Value)
            .Where(t =>
                (!tripIdSearch.HasValue && searchPattern == null)
                || (tripIdSearch.HasValue && t.Id == tripIdSearch.Value)
                || (searchPattern != null && (
                    EF.Functions.ILike(t.Id.ToString(), searchPattern) ||
                    (t.Contract != null && (
                        EF.Functions.ILike(t.Contract.ReferenceNumber, searchPattern) ||
                        EF.Functions.ILike(t.Contract.NameEn, searchPattern) ||
                        EF.Functions.ILike(t.Contract.NameAr, searchPattern))) ||
                    (t.VehicleAssignmentSnapshot != null && (
                        EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateNo, searchPattern) ||
                        EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateLetters, searchPattern) ||
                        EF.Functions.ILike(t.VehicleAssignmentSnapshot.HeadPlateNo + " " + t.VehicleAssignmentSnapshot.HeadPlateLetters, searchPattern))) ||
                    (t.Vehicle != null && (
                        EF.Functions.ILike(t.Vehicle.PlateNo, searchPattern) ||
                        EF.Functions.ILike(t.Vehicle.PlateLetters, searchPattern) ||
                        EF.Functions.ILike(t.Vehicle.PlateNo + " " + t.Vehicle.PlateLetters, searchPattern))) ||
                    (t.Driver != null && (
                        EF.Functions.ILike(t.Driver.FullName, searchPattern) ||
                        EF.Functions.ILike(t.Driver.FullNameAr, searchPattern) ||
                        EF.Functions.ILike(t.Driver.Phone, searchPattern))))));

        if (criteria.CurrentDriverStatus.HasValue)
            query = OperationalDriverStatusQueryFilter.Apply(query, criteria.CurrentDriverStatus.Value);

        if (criteria.LifecycleActor.HasValue)
            query = ApplyLifecycleActor(query, criteria.LifecycleActor.Value);

        return query;
    }

    private static IQueryable<Trip> ApplyLifecycleActor(IQueryable<Trip> query, TripLifecycleActor actor) =>
        actor switch
        {
            TripLifecycleActor.Driver => query.Where(t =>
                (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                && t.Recordings!.Any(r =>
                    (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                    && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED)
                && !t.Recordings!.Any(r =>
                    (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                    && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                    && r.AddedByModelType != "Driver")),
            TripLifecycleActor.Dispatcher => query.Where(t =>
                (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                && (!t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED)
                    || !t.Recordings!.Any(r =>
                        (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                        && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                        && r.AddedByModelType != "Admin"))),
            TripLifecycleActor.Assisted => query.Where(t =>
                (t.Status == TripStatus.InProgress || t.Status == TripStatus.Completed)
                && t.Recordings!.Any(r =>
                    (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                    && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                    && r.AddedByModelType == "Driver")
                && t.Recordings!.Any(r =>
                    (r.RecordableType == RecordableType.TRIP || r.RecordableType == RecordableType.TRIP_WAYPOINT)
                    && r.RecordableEventType <= RecordableEventType.WAYPOINT_UNLOADING_COMPLETED
                    && r.AddedByModelType == "Admin")),
            _ => query
        };

    private static string? LikePattern(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : "%" + value.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
}
