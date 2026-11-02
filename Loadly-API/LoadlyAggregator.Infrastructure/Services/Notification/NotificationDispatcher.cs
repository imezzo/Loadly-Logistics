using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Infrastructure.Services.Notification;


public class NotificationDispatcher : INotificationEventDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IServiceScopeFactory scopeFactory,
        IHostApplicationLifetime lifetime,
        ILogger<NotificationDispatcher> logger)
    {
        _scopeFactory = scopeFactory;
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task DispatchAsync(NotificationData data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Fire-and-forget: return to the originating request immediately and run recipient
        // resolution / preference + dedup gating / channel fan-out / in-app flush in the
        // background. The request's cancellationToken is intentionally NOT propagated — it is
        // cancelled when the HTTP response completes, which would abort the dispatch.
        _ = Task.Run(() => RunInBackgroundAsync(data));
        return Task.CompletedTask;
    }

    private async Task RunInBackgroundAsync(NotificationData data)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            await DispatchCoreAsync(scope.ServiceProvider, data, _lifetime.ApplicationStopping);
        }
        catch (OperationCanceledException)
        {
            // Application is shutting down; nothing to recover.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background notification dispatch failed for {DataType}.", data.GetType().Name);
        }
    }

    private async Task DispatchCoreAsync(IServiceProvider sp, NotificationData data, CancellationToken cancellationToken)
    {
        string dataTypeName = data.GetType().Name;

        INotificationHandlerAdapter? adapter = null;
        foreach (INotificationHandlerAdapter a in sp.GetServices<INotificationHandlerAdapter>())
        {
            if (a.CanHandle(data))
            {
                adapter = a;
                break;
            }
        }

        if (adapter is null)
        {
            _logger.LogError(
                "No INotificationHandlerAdapter registered for {DataType}. Add the corresponding NotificationHandlerAdapter registration in ServiceCollectionExtensions.",
                dataTypeName);
            return;
        }

        IReadOnlyList<NotificationRecipient> recipients;
        try
        {
            recipients = await adapter.BuildRecipientsAsync(data, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handler for {DataType} threw while building recipients.", dataTypeName);
            return;
        }

        _logger.LogInformation(
            "Dispatching {DataType} to {RecipientCount} recipient(s).",
            dataTypeName, recipients.Count);

        INotificationPipeline pipeline = sp.GetRequiredService<INotificationPipeline>();

        foreach (NotificationRecipient recipient in recipients)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await pipeline.DeliverToRecipientAsync(recipient, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to deliver {EventType} to {ModelType}:{ModelId} via pipeline.",
                    recipient.NotificationEventType, recipient.ModelType, recipient.ModelId);
            }
        }

        try
        {
            INotificationStore store = sp.GetRequiredService<INotificationStore>();
            await store.FlushAsync();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to flush buffered in-app notifications for {DataType}.", dataTypeName);
        }
    }
}
