namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// Base class for notification event payloads. Subclasses carry event-specific context
/// for channel fan-out — mirrors RecordingData for the delivery side.
/// </summary>
public abstract class NotificationData
{
}
