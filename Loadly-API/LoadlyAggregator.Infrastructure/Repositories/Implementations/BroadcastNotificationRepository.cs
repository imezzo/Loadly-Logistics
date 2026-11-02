using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class BroadcastNotificationRepository : Repository<BroadcastNotification>, IBroadcastNotificationRepository
{
    public BroadcastNotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<BroadcastNotification?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(b => b.Attachments)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<BroadcastNotification?> GetByPublicTokenAsync(string publicToken) =>
        await _dbSet
            .Include(b => b.Attachments)
            .FirstOrDefaultAsync(b => b.PublicToken == publicToken);

    public async Task<(List<BroadcastNotification> Items, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize)
    {
        IQueryable<BroadcastNotification> query = _dbSet.AsQueryable()
            .Where(b => string.IsNullOrWhiteSpace(search) ||
                b.Body.Contains(search!) ||
                (b.Subject != null && b.Subject.Contains(search!)) ||
                b.ModelType.Contains(search!));

        int totalCount = await query.CountAsync();

        List<BroadcastNotification> items = await query
            .Include(b => b.Attachments)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
