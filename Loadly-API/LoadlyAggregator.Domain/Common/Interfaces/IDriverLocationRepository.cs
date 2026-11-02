using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDriverLocationRepository : IRepository<DriverLocation>
{
    Task<DriverLocation?> GetByDriverIdAsync(long driverId);
    Task UpsertAsync(DriverLocation location);
}
