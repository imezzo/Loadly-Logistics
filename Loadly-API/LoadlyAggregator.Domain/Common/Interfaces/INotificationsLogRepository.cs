using System.Linq.Expressions;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationsLogRepository
{
    Task<NotificationsLog?> GetByIdAsync(long id);
    Task UpdateAsync(NotificationsLog entity);
    Task SaveChangesAsync();
    IQueryable<NotificationsLog> Query();
    Task<int> CountAsync(Expression<Func<NotificationsLog, bool>>? predicate = null);
    Task<List<NotificationsLog>> GetPaginatedAsync(
        Expression<Func<NotificationsLog, bool>>? predicate = null,
        int page = 1,
        int pageSize = 15);
    Task<int> CountWithPredicateAsync(Expression<Func<NotificationsLog, bool>>? predicate = null);
    Task<(List<NotificationsLog> Items, int TotalCount)> ListAsync(
        int? channel = null,
        int? status = null,
        string? recipient = null,
        string? modelType = null,
        long? modelId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 15);
}
