using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripBookingInvitationRoundRepository : Repository<TripBookingInvitationRound>, ITripBookingInvitationRoundRepository
{
    public TripBookingInvitationRoundRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TripBookingInvitationRound?> GetActiveByTripIdAsync(long tripId) =>
        await _dbSet
            .Where(r => r.TripId == tripId && r.Status == TripBookingInvitationRoundStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task CompleteActiveByTripIdAsync(long tripId, DateTime updatedAt) =>
        await _dbSet
            .Where(r => r.TripId == tripId && r.Status == TripBookingInvitationRoundStatus.Active)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Status, TripBookingInvitationRoundStatus.Completed)
                .SetProperty(r => r.UpdatedAt, updatedAt));
}
