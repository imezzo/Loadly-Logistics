using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripWaypointRepository : Repository<TripWaypoint>, ITripWaypointRepository
{
    public TripWaypointRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TripWaypoint>> GetByTripIdAsync(long tripId) =>
        await _dbSet.Where(w => w.TripId == tripId).OrderBy(w => w.Sequence).ToListAsync();
}
