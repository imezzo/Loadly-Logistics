using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripBookingRequestRepository : Repository<TripBookingRequest>, ITripBookingRequestRepository
{
    public TripBookingRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<TripBookingRequest?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(r => r.Contractor)
            .Include(r => r.ReviewedByAdmin)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Contract!)
                    .ThenInclude(c => c.Company)
                        .ThenInclude(c => c!.CompanyDetail)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractDestinationAssignment!)
                    .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractDestinationAssignment!)
                    .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractVehicleType!)
                    .ThenInclude(vt => vt.VehicleType)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Lead)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Waypoints)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<TripBookingRequest> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("trips.booking_request_not_found_with_id", id);

    public async Task<TripBookingRequest?> GetByTripAndContractorAsync(long tripId, long contractorId) =>
        await _dbSet
            .Where(r => r.TripId == tripId && r.ContractorId == contractorId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<TripBookingRequest?> GetByRoundAndContractorAsync(long roundId, long contractorId) =>
        await _dbSet
            .FirstOrDefaultAsync(r => r.TripBookingInvitationRoundId == roundId && r.ContractorId == contractorId);

    public async Task<TripBookingRequest?> GetPendingByTripAndContractorAsync(long tripId, long contractorId) =>
        await _dbSet
            .Where(r => r.TripId == tripId
                && r.ContractorId == contractorId
                && r.Status == TripBookingRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<TripBookingRequest?> GetPendingByRoundAndContractorAsync(long roundId, long contractorId) =>
        await _dbSet
            .FirstOrDefaultAsync(r => r.TripBookingInvitationRoundId == roundId
                && r.ContractorId == contractorId
                && r.Status == TripBookingRequestStatus.Pending);

    public async Task<List<TripBookingRequest>> ListByTripIdAsync(long tripId) =>
        await _dbSet
            .AsNoTracking()
            .Include(r => r.Contractor)
            .Include(r => r.ReviewedByAdmin)
            .Where(r => r.TripId == tripId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<List<TripBookingRequest>> ListPendingByTripIdAsync(long tripId) =>
        await _dbSet
            .Where(r => r.TripId == tripId && r.Status == TripBookingRequestStatus.Pending)
            .ToListAsync();

    public async Task<(List<TripBookingRequest> Items, int TotalCount)> ListByContractorIdAsync(
        long contractorId,
        TripBookingRequestStatus? status,
        long? tripId,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        long? vehicleTypeId,
        string? search,
        int page,
        int pageSize)
    {
        long? tripIdSearch = search?.Trim() is { } trimmed
            && trimmed.StartsWith(TripReferenceNumber.Prefix, StringComparison.OrdinalIgnoreCase)
            && long.TryParse(trimmed.AsSpan(TripReferenceNumber.Prefix.Length), out var parsedTripId)
            && parsedTripId > 0
            ? parsedTripId
            : null;

        var searchPattern = tripIdSearch.HasValue || string.IsNullOrWhiteSpace(search)
            ? null
            : "%" + EscapeForILike(search.Trim()) + "%";

        var query = _dbSet
            .AsNoTracking()
            .Include(r => r.ReviewedByAdmin)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Contract!)
                    .ThenInclude(c => c.Company)
                        .ThenInclude(c => c!.CompanyDetail)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractDestinationAssignment!)
                    .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.StartWaypointNavigation)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractDestinationAssignment!)
                    .ThenInclude(tcd => tcd.Destination)
                    .ThenInclude(d => d.EndWaypointNavigation)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.ContractVehicleType!)
                    .ThenInclude(vt => vt.VehicleType)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Lead)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Waypoints)
            .Include(r => r.Trip!)
                .ThenInclude(t => t.Driver)
            .Where(r => r.ContractorId == contractorId)
            .Where(r => !status.HasValue || r.Status == status.Value)
            .Where(r => !tripId.HasValue || r.TripId == tripId.Value)
            .Where(r => r.Status != TripBookingRequestStatus.Pending
                || (r.TripBookingInvitationRound != null
                    && r.TripBookingInvitationRound.Status == TripBookingInvitationRoundStatus.Active))
            .Where(r => !startDateFrom.HasValue || (r.Trip != null && r.Trip.StartDate >= startDateFrom.Value))
            .Where(r => !startDateTo.HasValue || (r.Trip != null && r.Trip.StartDate <= startDateTo.Value))
            .Where(r => !vehicleTypeId.HasValue ||
                (r.Trip != null
                 && r.Trip.ContractVehicleType != null
                 && r.Trip.ContractVehicleType.VehicleTypeId == vehicleTypeId.Value))
            .Where(r =>
                (!tripIdSearch.HasValue && searchPattern == null)
                || (tripIdSearch.HasValue && r.Trip != null && r.Trip.Id == tripIdSearch.Value)
                || (searchPattern != null && r.Trip != null && (
                    EF.Functions.ILike(r.Trip.Id.ToString(), searchPattern)
                    || (r.Trip.Driver != null && (
                        EF.Functions.ILike(r.Trip.Driver.FullName, searchPattern)
                        || EF.Functions.ILike(r.Trip.Driver.FullNameAr, searchPattern)
                        || EF.Functions.ILike(r.Trip.Driver.Phone, searchPattern))))));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task RejectPendingByTripIdAsync(long tripId, long? reviewedByAdminId, DateTime reviewedAt, long? exceptRequestId = null) =>
        await _dbSet
            .Where(r => r.TripId == tripId
                && r.Status == TripBookingRequestStatus.Pending
                && (!exceptRequestId.HasValue || r.Id != exceptRequestId.Value))
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, TripBookingRequestStatus.Rejected)
                .SetProperty(r => r.ReviewedAt, reviewedAt)
                .SetProperty(r => r.ReviewedByAdminId, reviewedByAdminId)
                .SetProperty(r => r.UpdatedAt, reviewedAt));

    private static string EscapeForILike(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
