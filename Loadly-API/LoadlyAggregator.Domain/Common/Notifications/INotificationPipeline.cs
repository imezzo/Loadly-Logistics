namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// Delivers a single recipient through the notification pipeline:
/// preference gate -> dedup gate -> channel router.
/// </summary>
public interface INotificationPipeline
{
    Task DeliverToRecipientAsync(NotificationRecipient recipient, CancellationToken cancellationToken = default);
}
