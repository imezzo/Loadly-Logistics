namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// High-level entry point for business-event notifications.
/// Actions create a typed <see cref="NotificationData"/> subclass and hand it here;
/// the implementation resolves recipients, applies preferences / dedup, and fans out across channels.
/// For direct, transactional sends (OTP, password-reset) use <see cref="INotificationTransport"/> instead.
/// </summary>
public interface INotificationEventDispatcher
{
    Task DispatchAsync(NotificationData data, CancellationToken cancellationToken = default);
}
