using LoadlyAggregator.Domain.Common.Notifications.Channels;

namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripScheduledNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public long? CompanyId { get; init; }
    public long? DriverId { get; init; }
    public TripScheduleDriverPushKind? DriverPushKind { get; init; }
}
