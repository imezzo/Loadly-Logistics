namespace LoadlyAggregator.Domain.Common.Notifications.EventData;

/// <summary>
/// Event data published when a trip is cancelled (admin or company actor).
/// Drives push + in-app notification to the company (when present), the assigned driver
/// (when the trip had reached an assigned status and a driver was on it), and the assigned contractor.
/// </summary>
public class TripCancelledNotificationData : NotificationData
{
    public required long TripId { get; init; }
    public long? CompanyId { get; init; }
    public long? DriverId { get; init; }
    public long? ContractorId { get; init; }
}
