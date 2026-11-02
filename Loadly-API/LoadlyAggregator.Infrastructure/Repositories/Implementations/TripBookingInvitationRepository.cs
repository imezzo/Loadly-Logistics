using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripBookingInvitationRepository : Repository<TripBookingInvitation>, ITripBookingInvitationRepository
{
    public TripBookingInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<TripBookingInvitation>> ListByTripIdAsync(long tripId) =>
        await _dbSet
            .AsNoTracking()
            .Include(i => i.Contractor)
            .Where(i => i.TripId == tripId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task<List<TripBookingInvitation>> ListByContractorIdAsync(long contractorId) =>
        await _dbSet
            .AsNoTracking()
            .Include(i => i.Trip)
                .ThenInclude(t => t!.Contract)
            .Include(i => i.Trip!)
                .ThenInclude(t => t.ContractDestinationAssignment!)
                    .ThenInclude(tcd => tcd.Destination)
            .Include(i => i.Trip!)
                .ThenInclude(t => t.ContractVehicleType)
            .Where(i => i.ContractorId == contractorId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task<TripBookingInvitation?> GetByTripAndContractorAsync(long tripId, long contractorId) =>
        await _dbSet
            .Where(i => i.TripId == tripId && i.ContractorId == contractorId)
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<TripBookingInvitation?> GetByRoundAndContractorAsync(long roundId, long contractorId) =>
        await _dbSet
            .FirstOrDefaultAsync(i => i.TripBookingInvitationRoundId == roundId && i.ContractorId == contractorId);

    public Task ResolveOnBookingApprovalAsync(long tripId, long acceptedContractorId, DateTime updatedAt, long? roundId = null)
    {
        var query = _dbSet.Where(i => i.TripId == tripId && i.Status == TripBookingInvitationStatus.Pending);
        if (roundId.HasValue)
            query = query.Where(i => i.TripBookingInvitationRoundId == roundId.Value);

        return query.ExecuteUpdateAsync(s => s
            .SetProperty(
                i => i.Status,
                i => i.ContractorId == acceptedContractorId
                    ? TripBookingInvitationStatus.Accepted
                    : TripBookingInvitationStatus.Cancelled)
            .SetProperty(i => i.UpdatedAt, updatedAt));
    }

    public async Task CancelPendingByTripIdAsync(long tripId, DateTime updatedAt) =>
        await _dbSet
            .Where(i => i.TripId == tripId && i.Status == TripBookingInvitationStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Status, TripBookingInvitationStatus.Cancelled)
                .SetProperty(i => i.UpdatedAt, updatedAt));
}
