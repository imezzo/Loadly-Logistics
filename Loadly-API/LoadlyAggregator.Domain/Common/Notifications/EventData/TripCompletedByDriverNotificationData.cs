namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripCompletedByDriverNotificationData : NotificationData
{
    public required long TripId { get; init; }
}
