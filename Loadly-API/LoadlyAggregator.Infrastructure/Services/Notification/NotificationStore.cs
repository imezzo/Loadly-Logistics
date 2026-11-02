using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using NotificationEntity = LoadlyAggregator.Domain.Common.Entities.Notification;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class NotificationStore : INotificationStore
{
    private readonly INotificationRepository _notificationRepository;
    private readonly List<NotificationEntity> _pending = new();

    public NotificationStore(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task StoreAsync(long modelId, string modelType, string title, string body,
        NotificationEventType? eventType = null,
        NotificationResourceType? resourceType = null, long? resourceId = null, string? clickAction = null)
    {
        Enqueue(modelId, modelType, title, body, eventType, resourceType, resourceId, clickAction);
        await FlushAsync();
    }

    public void Enqueue(long modelId, string modelType, string title, string body,
        NotificationEventType? eventType = null,
        NotificationResourceType? resourceType = null, long? resourceId = null, string? clickAction = null)
    {
        _pending.Add(new NotificationEntity
        {
            ModelId = modelId,
            ModelType = modelType,
            Title = title,
            Body = body,
            EventType = eventType,
            ResourceType = resourceType,
            ResourceId = resourceId,
            ClickAction = clickAction
        });
    }

    public async Task FlushAsync()
    {
        if (_pending.Count == 0)
            return;

        await _notificationRepository.AddRangeAsync(_pending);
        await _notificationRepository.SaveChangesAsync();
        _pending.Clear();
    }
}
