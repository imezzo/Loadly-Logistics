namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// Builds the list of recipients + messages for a specific notification event.
/// One handler per NotificationData subclass; lives in Infrastructure since it
/// may need repos, template services, etc.
/// </summary>
public interface INotificationHandler<in TData> where TData : NotificationData
{
    Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(TData data, CancellationToken cancellationToken = default);
}
