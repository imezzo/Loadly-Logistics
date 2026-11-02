namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripPushedToContractorsNotificationData : NotificationData
{
    public required long ContractorId { get; init; }
    public required int TripCount { get; init; }
}
