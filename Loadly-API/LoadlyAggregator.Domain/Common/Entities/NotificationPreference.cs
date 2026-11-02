using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class NotificationPreference : BaseEntity
{
    public long ModelId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public NotificationEventType EventType { get; set; }
    public bool IsEnabled { get; set; } = true;
}
