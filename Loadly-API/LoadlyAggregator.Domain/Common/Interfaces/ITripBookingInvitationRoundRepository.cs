using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripBookingInvitationRoundRepository : IRepository<TripBookingInvitationRound>
{
    Task<TripBookingInvitationRound?> GetActiveByTripIdAsync(long tripId);
    Task CompleteActiveByTripIdAsync(long tripId, DateTime updatedAt);
}
