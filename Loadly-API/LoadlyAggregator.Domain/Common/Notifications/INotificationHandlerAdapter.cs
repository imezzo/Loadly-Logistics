namespace LoadlyAggregator.Domain.Common.Notifications;

/// <summary>
/// Non-generic adapter over <see cref="INotificationHandler{TData}"/>.
/// Allows <see cref="INotificationEventDispatcher"/> implementations to resolve all handlers as
/// <c>IEnumerable&lt;INotificationHandlerAdapter&gt;</c> — no reflection, no Service Locator.
/// </summary>
public interface INotificationHandlerAdapter
{
    bool CanHandle(NotificationData data);
    Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(NotificationData data, CancellationToken cancellationToken = default);
}
