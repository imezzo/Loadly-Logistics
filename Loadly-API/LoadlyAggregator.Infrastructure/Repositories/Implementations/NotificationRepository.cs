using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Notification> Items, int TotalCount)> ListAsync(
        long modelId,
        string modelType,
        NotificationEventType? eventType,
        bool? isRead,
        int page,
        int pageSize,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        NotificationEventType? excludeEventType = null)
    {
        modelType = NormalizeModelType(modelType);

        var query = _dbSet
            .Where(n => n.ModelId == modelId && n.ModelType == modelType)
            .Where(n => !eventType.HasValue || n.EventType == eventType.Value)
            .Where(n => !excludeEventType.HasValue || n.EventType != excludeEventType.Value)
            .Where(n => !isRead.HasValue || (isRead.Value ? n.ReadAt != null : n.ReadAt == null))
            .Where(n => !createdFrom.HasValue || n.CreatedAt >= createdFrom.Value)
            .Where(n => !createdTo.HasValue || n.CreatedAt <= createdTo.Value);

        var totalCount = await query.CountAsync();
        List<Notification> items = await query
            .AsNoTracking()
            .AsSplitQuery()
            .Include(n => n.BroadcastNotification!)
                .ThenInclude(b => b.Attachments)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetUnreadCountAsync(
        long modelId,
        string modelType,
        NotificationEventType? eventType = null,
        NotificationEventType? excludeEventType = null)
    {
        modelType = NormalizeModelType(modelType);

        return await _dbSet
            .Where(n => n.ModelId == modelId && n.ModelType == modelType && n.ReadAt == null)
            .Where(n => !eventType.HasValue || n.EventType == eventType.Value)
            .Where(n => !excludeEventType.HasValue || n.EventType != excludeEventType.Value)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(long id, long modelId, string modelType)
    {
        modelType = NormalizeModelType(modelType);

        var now = DateTime.UtcNow;
        await _dbSet
            .Where(n => n.Id == id && n.ModelId == modelId && n.ModelType == modelType && n.ReadAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.ReadAt, now)
                .SetProperty(n => n.UpdatedAt, now));
    }

    public async Task MarkAsReadAsync(IReadOnlyList<long> ids, long modelId, string modelType)
    {
        if (ids == null || ids.Count == 0)
            return;

        modelType = NormalizeModelType(modelType);

        var now = DateTime.UtcNow;
        await _dbSet
            .Where(n => ids.Contains(n.Id) && n.ModelId == modelId && n.ModelType == modelType && n.ReadAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.ReadAt, now)
                .SetProperty(n => n.UpdatedAt, now));
    }

    public async Task MarkAllAsReadAsync(long modelId, string modelType)
    {
        modelType = NormalizeModelType(modelType);

        var now = DateTime.UtcNow;
        await _dbSet
            .Where(n => n.ModelId == modelId && n.ModelType == modelType && n.ReadAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.ReadAt, now)
                .SetProperty(n => n.UpdatedAt, now));
    }

    private static string NormalizeModelType(string modelType) =>
        string.IsNullOrEmpty(modelType) ? modelType : char.ToUpper(modelType[0]) + modelType[1..];
}
