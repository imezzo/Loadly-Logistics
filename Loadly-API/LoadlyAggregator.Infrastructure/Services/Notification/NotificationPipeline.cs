using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class NotificationPipeline : INotificationPipeline
{
    private readonly ILogger<NotificationPipeline> _logger;
    private readonly INotificationPreferenceService _preferenceService;
    private readonly INotificationDeduplicationService? _deduplicationService;
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;
    private readonly IPushSender _pushSender;
    private readonly INotificationStore _notificationStore;

    public NotificationPipeline(
        ILogger<NotificationPipeline> logger,
        INotificationPreferenceService preferenceService,
        IEmailSender emailSender,
        ISmsSender smsSender,
        IPushSender pushSender,
        INotificationStore notificationStore,
        INotificationDeduplicationService? deduplicationService = null)
    {
        _logger = logger;
        _preferenceService = preferenceService;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _pushSender = pushSender;
        _notificationStore = notificationStore;
        _deduplicationService = deduplicationService;
    }

    public async Task DeliverToRecipientAsync(NotificationRecipient recipient, CancellationToken cancellationToken = default)
    {
        foreach (NotificationChannel channel in recipient.Channels)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await PassesPreferenceGateAsync(recipient, channel))
                continue;

            if (!await PassesDedupGateAsync(recipient, channel))
                continue;

            await RouteToChannelAsync(recipient, channel, cancellationToken);
        }
    }

    private async Task<bool> PassesPreferenceGateAsync(NotificationRecipient recipient, NotificationChannel channel)
    {
        bool shouldSend = await _preferenceService.ShouldSendAsync(
            recipient.ModelId, recipient.ModelType, channel, recipient.NotificationEventType);

        if (!shouldSend)
        {
            _logger.LogInformation(
                "Preference disabled for {EventType}/{Channel} on {ModelType}:{ModelId}. Skipping.",
                recipient.NotificationEventType, channel, recipient.ModelType, recipient.ModelId);
        }

        return shouldSend;
    }

    private async Task<bool> PassesDedupGateAsync(NotificationRecipient recipient, NotificationChannel channel)
    {
        if (_deduplicationService is null)
            return true;

        string recipientKey = channel switch
        {
            NotificationChannel.EMAIL => recipient.EmailAddress ?? $"user:{recipient.ModelId}",
            NotificationChannel.SMS => recipient.PhoneNumber ?? $"user:{recipient.ModelId}",
            NotificationChannel.PUSH => $"user:{recipient.ModelId}:{recipient.ModelType.ToLowerInvariant()}",
            NotificationChannel.IN_APP => $"inapp:{recipient.ModelId}:{recipient.ModelType.ToLowerInvariant()}",
            _ => $"user:{recipient.ModelId}"
        };

        bool acquired = await _deduplicationService.TryAcquireAsync(
            channel, recipient.ModelId, recipient.ModelType, recipient.NotificationEventType, recipientKey,
            resourceId: recipient.ResourceId);

        if (!acquired)
        {
            _logger.LogDebug(
                "Dedup: skipping duplicate {EventType}/{Channel} for {ModelType}:{ModelId}, resource {ResourceId}.",
                recipient.NotificationEventType, channel, recipient.ModelType, recipient.ModelId, recipient.ResourceId);
        }

        return acquired;
    }

    private Task RouteToChannelAsync(NotificationRecipient recipient, NotificationChannel channel, CancellationToken cancellationToken) =>
        channel switch
        {
            NotificationChannel.EMAIL => SendEmailAsync(recipient, cancellationToken),
            NotificationChannel.SMS => SendSmsAsync(recipient, cancellationToken),
            NotificationChannel.PUSH => SendPushAsync(recipient, cancellationToken),
            NotificationChannel.IN_APP => StoreInAppAsync(recipient),
            _ => Task.CompletedTask
        };

    private async Task SendEmailAsync(NotificationRecipient recipient, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipient.EmailAddress))
        {
            _logger.LogDebug("No email address for {ModelType}:{ModelId}. Skipping email.", recipient.ModelType, recipient.ModelId);
            return;
        }

        await _emailSender.SendAsync(new EmailMessage(
            To: recipient.EmailAddress,
            ToName: string.Empty,
            Subject: recipient.EmailSubject ?? recipient.Title,
            Body: recipient.EmailHtmlBody ?? recipient.Body,
            IsHtml: recipient.EmailHtmlBody is not null,
            Domain: recipient.Domain,
            EventType: recipient.NotificationEventType), cancellationToken);
    }

    private async Task SendSmsAsync(NotificationRecipient recipient, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipient.PhoneNumber))
        {
            _logger.LogDebug("No phone number for {ModelType}:{ModelId}. Skipping SMS.", recipient.ModelType, recipient.ModelId);
            return;
        }

        await _smsSender.SendAsync(new SmsMessage(
            PhoneNumber: recipient.PhoneNumber,
            Message: recipient.Body,
            Domain: recipient.Domain,
            EventType: recipient.NotificationEventType), cancellationToken);
    }

    private async Task SendPushAsync(NotificationRecipient recipient, CancellationToken cancellationToken)
    {
        await _pushSender.SendAsync(new PushUserMessage(
            ModelId: recipient.ModelId,
            ModelType: recipient.ModelType,
            Title: recipient.Title,
            Body: recipient.Body,
            Domain: recipient.Domain,
            EventType: recipient.NotificationEventType,
            Data: recipient.PushData,
            ClickAction: recipient.PushClickAction), cancellationToken);
    }

    private Task StoreInAppAsync(NotificationRecipient recipient)
    {
        _notificationStore.Enqueue(
            recipient.ModelId,
            recipient.ModelType,
            recipient.Title,
            recipient.Body,
            recipient.NotificationEventType,
            recipient.ResourceType,
            recipient.ResourceId,
            recipient.ClickAction);
        return Task.CompletedTask;
    }
}
