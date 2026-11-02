using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripWaypointRepository : IRepository<TripWaypoint>
{
    Task<IEnumerable<TripWaypoint>> GetByTripIdAsync(long tripId);
}
