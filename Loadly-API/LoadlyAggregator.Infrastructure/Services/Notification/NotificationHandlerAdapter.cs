using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

/// <summary>
/// Wraps a typed <see cref="INotificationHandler{TData}"/> as a non-generic
/// <see cref="INotificationHandlerAdapter"/>, eliminating the need for reflection
/// or a Service Locator in the admin.
/// </summary>
public sealed class NotificationHandlerAdapter<TData> : INotificationHandlerAdapter
    where TData : NotificationData
{
    private readonly INotificationHandler<TData> _inner;

    public NotificationHandlerAdapter(INotificationHandler<TData> inner) => _inner = inner;

    public bool CanHandle(NotificationData data) => data is TData;

    public Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(NotificationData data, CancellationToken cancellationToken = default)
        => _inner.BuildRecipientsAsync((TData)data, cancellationToken);
}
