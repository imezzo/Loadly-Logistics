using FlexLabs.EntityFrameworkCore.Upsert;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DriverLocationRepository : Repository<DriverLocation>, IDriverLocationRepository
{
    public DriverLocationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DriverLocation?> GetByDriverIdAsync(long driverId)
    {
        return await _dbSet
            .Where(dl => dl.DriverId == driverId)
            .FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(DriverLocation location)
    {
        var now = DateTime.UtcNow;

        location.CreatedAt = now;
        location.UpdatedAt = now;

        await _context.Set<DriverLocation>()
            .Upsert(location)
            .On(dl => dl.DriverId)
            .WhenMatched(dl => new DriverLocation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = location.Accuracy,
                Heading = location.Heading,
                Speed = location.Speed,
                RecordedAt = location.RecordedAt,
                UpdatedAt = now
            })
            .RunAsync();
    }
}
