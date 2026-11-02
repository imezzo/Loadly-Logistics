namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

public class TripBookingRequestApprovedNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public required long ContractorId { get; init; }
    public required long BookingRequestId { get; init; }
}
