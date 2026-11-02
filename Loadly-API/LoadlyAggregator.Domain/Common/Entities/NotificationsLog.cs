using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class NotificationsLog : BaseEntity
{
    public string Recipient { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public string? Response { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.PENDING;
    public NotificationEventType? EventType { get; set; }
    public long? ModelId { get; set; }
    public string? ModelType { get; set; }
    public long? BroadcastNotificationId { get; set; }
}
