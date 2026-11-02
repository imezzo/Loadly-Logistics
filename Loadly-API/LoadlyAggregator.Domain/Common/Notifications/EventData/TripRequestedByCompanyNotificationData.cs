namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripRequestedByCompanyNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public required long CompanyId { get; init; }
}
