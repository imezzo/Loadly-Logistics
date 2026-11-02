using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripBookingInvitationRepository : IRepository<TripBookingInvitation>
{
    Task<List<TripBookingInvitation>> ListByTripIdAsync(long tripId);
    Task<List<TripBookingInvitation>> ListByContractorIdAsync(long contractorId);
    Task<TripBookingInvitation?> GetByTripAndContractorAsync(long tripId, long contractorId);
    Task<TripBookingInvitation?> GetByRoundAndContractorAsync(long roundId, long contractorId);
    Task ResolveOnBookingApprovalAsync(long tripId, long acceptedContractorId, DateTime updatedAt, long? roundId = null);
    Task CancelPendingByTripIdAsync(long tripId, DateTime updatedAt);
}
