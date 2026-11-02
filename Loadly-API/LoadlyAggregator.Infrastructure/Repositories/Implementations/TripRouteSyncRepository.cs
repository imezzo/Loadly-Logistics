using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripRouteSyncRepository : Repository<TripRouteSync>, ITripRouteSyncRepository
{
    public TripRouteSyncRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TripRouteSync?> GetByTripIdAsync(long tripId) =>
        await _dbSet
            .Where(s => s.TripId == tripId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
}
