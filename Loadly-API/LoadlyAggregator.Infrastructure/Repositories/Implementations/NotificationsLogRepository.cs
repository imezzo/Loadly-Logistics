using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class NotificationsLogRepository : INotificationsLogRepository
{
    private readonly LogsDbContext _context;
    private readonly DbSet<NotificationsLog> _dbSet;

    public NotificationsLogRepository(LogsDbContext context)
    {
        _context = context;
        _dbSet = context.NotificationsLogs;
    }

    public async Task<NotificationsLog?> GetByIdAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

    public Task UpdateAsync(NotificationsLog entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();

    public IQueryable<NotificationsLog> Query() =>
        _dbSet.AsQueryable().OrderByDescending(x => x.CreatedAt);

    public async Task<int> CountAsync(Expression<Func<NotificationsLog, bool>>? predicate = null) =>
        await (predicate != null ? _dbSet.Where(predicate) : _dbSet.AsQueryable())
            .CountAsync();

    public async Task<List<NotificationsLog>> GetPaginatedAsync(
        Expression<Func<NotificationsLog, bool>>? predicate = null,
        int page = 1,
        int pageSize = 15) =>
        await (predicate != null ? _dbSet.Where(predicate) : _dbSet.AsQueryable())
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> CountWithPredicateAsync(Expression<Func<NotificationsLog, bool>>? predicate = null) =>
        await CountAsync(predicate);

    public async Task<(List<NotificationsLog> Items, int TotalCount)> ListAsync(
        int? channel = null,
        int? status = null,
        string? recipient = null,
        string? modelType = null,
        long? modelId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 15)
    {
        int normalizedPage = page > 0 ? page : 1;
        int normalizedPageSize = pageSize > 0 ? pageSize : 15;
        IQueryable<NotificationsLog> query = _dbSet.AsQueryable()
            .Where(x => !channel.HasValue || x.Channel == (NotificationChannel)channel.Value)
            .Where(x => !status.HasValue || x.Status == (NotificationStatus)status.Value)
            .Where(x => string.IsNullOrWhiteSpace(recipient) || x.Recipient.Contains(recipient!))
            .Where(x => string.IsNullOrWhiteSpace(modelType) || x.ModelType == modelType)
            .Where(x => !modelId.HasValue || x.ModelId == modelId.Value)
            .Where(x => !startDate.HasValue || x.CreatedAt >= startDate.Value)
            .Where(x => !endDate.HasValue || x.CreatedAt <= endDate.Value);
        int totalCount = await query.CountAsync();
        List<NotificationsLog> items = await query
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();
        return (items, totalCount);
    }
}

