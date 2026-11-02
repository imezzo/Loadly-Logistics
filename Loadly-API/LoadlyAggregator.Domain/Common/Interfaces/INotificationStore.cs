using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface INotificationStore
{
    Task StoreAsync(long modelId, string modelType, string title, string body,
        NotificationEventType? eventType = null,
        NotificationResourceType? resourceType = null, long? resourceId = null, string? clickAction = null);

    void Enqueue(long modelId, string modelType, string title, string body,
        NotificationEventType? eventType = null,
        NotificationResourceType? resourceType = null, long? resourceId = null, string? clickAction = null);

    Task FlushAsync();
}
