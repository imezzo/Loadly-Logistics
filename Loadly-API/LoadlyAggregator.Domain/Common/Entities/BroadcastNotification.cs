using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class BroadcastNotification : BaseEntity
{
    public long AddedBy { get; set; }
    public BroadcastChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public string? ModelIds { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.PENDING;
    public string? PublicToken { get; set; }

    public virtual ICollection<BroadcastNotificationAttachment> Attachments { get; set; } = new List<BroadcastNotificationAttachment>();
}

