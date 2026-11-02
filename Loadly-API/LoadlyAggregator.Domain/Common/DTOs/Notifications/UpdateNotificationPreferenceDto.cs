using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Notifications;

public class UpdateNotificationPreferenceDto
{
    public NotificationChannel Channel { get; set; }
    public NotificationEventType EventType { get; set; }
    public bool IsEnabled { get; set; }
}
