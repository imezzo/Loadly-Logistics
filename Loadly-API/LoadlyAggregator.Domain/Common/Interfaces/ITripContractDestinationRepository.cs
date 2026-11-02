using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripContractDestinationRepository : IRepository<TripContractDestination>
{
    Task<IReadOnlyList<TripContractDestination>> GetByTripIdAsync(long tripId);
    Task<IReadOnlyList<TripContractDestination>> GetByDestinationIdAsync(long destinationId);
}
