using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IWaypointRepository : IRepository<Waypoint>
{
    Task<Waypoint> GetByIdOrThrowAsync(long id);
}
