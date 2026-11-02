using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class BroadcastNotificationAttachment : BaseEntity
{
    public long BroadcastNotificationId { get; set; }
    public string Path { get; set; } = string.Empty;

    public virtual BroadcastNotification BroadcastNotification { get; set; } = null!;
}
