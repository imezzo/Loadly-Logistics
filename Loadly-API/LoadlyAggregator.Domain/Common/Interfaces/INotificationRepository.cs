using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationRepository : IRepository<Common.Entities.Notification>
{
    Task<(List<Common.Entities.Notification> Items, int TotalCount)> ListAsync(
        long modelId,
        string modelType,
        NotificationEventType? eventType,
        bool? isRead,
        int page,
        int pageSize,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        NotificationEventType? excludeEventType = null);

    Task<int> GetUnreadCountAsync(
        long modelId,
        string modelType,
        NotificationEventType? eventType = null,
        NotificationEventType? excludeEventType = null);
    Task MarkAsReadAsync(long id, long modelId, string modelType);
    Task MarkAsReadAsync(IReadOnlyList<long> ids, long modelId, string modelType);
    Task MarkAllAsReadAsync(long modelId, string modelType);
}
