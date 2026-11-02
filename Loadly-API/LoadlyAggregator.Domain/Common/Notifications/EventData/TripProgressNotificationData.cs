using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripProgressNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public required NotificationEventType NotificationEventType { get; init; }
    public long? WaypointId { get; init; }
    public string? WaypointName { get; init; }
}
