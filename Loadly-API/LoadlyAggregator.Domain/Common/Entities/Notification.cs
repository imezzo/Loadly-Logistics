using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class Notification : BaseEntity
{
    public long ModelId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationEventType? EventType { get; set; }
    public NotificationResourceType? ResourceType { get; set; }
    public long? ResourceId { get; set; }
    public string? ClickAction { get; set; }
    public DateTime? ReadAt { get; set; }
    public long? BroadcastNotificationId { get; set; }
    public virtual BroadcastNotification? BroadcastNotification { get; set; }
}
