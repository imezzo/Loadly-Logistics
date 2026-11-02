using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class WaypointRepository : Repository<Waypoint>, IWaypointRepository
{
    public WaypointRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Waypoint> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(w => w.Id == id)
        ?? throw new NotFoundException("waypoints.not_found_with_id", id);
}
