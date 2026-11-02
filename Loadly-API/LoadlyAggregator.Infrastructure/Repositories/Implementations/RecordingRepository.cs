using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class RecordingRepository(ApplicationDbContext context) : IRecordingRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Recording> _dbSet = context.Set<Recording>();

    public async Task AddAsync(Recording recording)
    {
        await _dbSet.AddAsync(recording);
    }

    public async Task<IEnumerable<Recording>> GetTimelineByRecordableAsync(
        long recordableId,
        IEnumerable<RecordableType> recordableTypes,
        int? limit = null)
    {
        var query = _dbSet
            .Where(r => r.RecordableId == recordableId
                && recordableTypes.Contains(r.RecordableType))
            .OrderByDescending(r => r.CreatedAt);

        var results = limit.HasValue
            ? await query.Take(limit.Value).ToListAsync()
            : await query.ToListAsync();
        return results;
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
