using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripRouteSyncRepository : IRepository<TripRouteSync>
{
    Task<TripRouteSync?> GetByTripIdAsync(long tripId);
}
