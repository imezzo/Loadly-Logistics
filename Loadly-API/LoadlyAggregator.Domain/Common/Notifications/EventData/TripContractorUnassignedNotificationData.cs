namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripContractorUnassignedNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public required long ContractorId { get; init; }
}
