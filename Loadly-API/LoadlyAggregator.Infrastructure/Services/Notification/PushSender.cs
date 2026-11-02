using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Queue;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class PushSender : IPushSender
{
    private readonly ILogger<PushSender> _logger;
    private readonly IQueueService _queueService;
    private readonly IQueueNameResolver _queueNameResolver;
    private readonly IDeviceTokenRepository _deviceTokenRepository;

    public PushSender(
        ILogger<PushSender> logger,
        IQueueService queueService,
        IQueueNameResolver queueNameResolver,
        IDeviceTokenRepository deviceTokenRepository)
    {
        _logger = logger;
        _queueService = queueService;
        _queueNameResolver = queueNameResolver;
        _deviceTokenRepository = deviceTokenRepository;
    }

    public Task SendAsync(PushUserMessage message, CancellationToken cancellationToken = default) =>
        ResolveAndSendAsync(message.ModelId, message.ModelType, message.Title, message.Body, message.Data, message.ClickAction, message.Domain, message.EventType);

    public Task<bool> SendToDeviceAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null, string? imageUrl = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        EnqueueDevicePushAsync(deviceToken, title, body, data, imageUrl, clickAction, domain, modelId, modelType, Enum.TryParse<NotificationEventType>(eventType, ignoreCase: true, out var et1) ? et1 : null);

    public Task SendToUserAsync(long modelId, string modelType, string title, string body, Dictionary<string, string>? data = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, string? eventType = null, long? broadcastNotificationId = null) =>
        ResolveAndSendAsync(modelId, modelType, title, body, data, clickAction, domain, Enum.TryParse<NotificationEventType>(eventType, ignoreCase: true, out var et2) ? et2 : null, broadcastNotificationId);

    private async Task ResolveAndSendAsync(long modelId, string modelType, string title, string body, Dictionary<string, string>? data, string? clickAction, NotificationDomain domain, NotificationEventType? eventType = null, long? broadcastNotificationId = null)
    {
        if (string.IsNullOrWhiteSpace(modelType))
        {
            _logger.LogWarning("Push send skipped: modelType is empty.");
            return;
        }

        string deviceUserType = modelType.ToLowerInvariant();
        List<DeviceToken> deviceTokens = await _deviceTokenRepository.GetActiveTokensByUserAsync(modelId, deviceUserType);
        var validTokens = deviceTokens.Where(t => !string.IsNullOrWhiteSpace(t.FcmToken)).ToList();

        if (validTokens.Count == 0)
        {
            _logger.LogDebug("No valid device tokens for user {ModelId} ({DeviceUserType}). Skipping push.", modelId, deviceUserType);
            return;
        }

        Task<bool>[] sendTasks = validTokens.Select(token =>
            EnqueueDevicePushAsync(token.FcmToken!, title, body, data, null, clickAction, domain, modelId, modelType, eventType, broadcastNotificationId)).ToArray();

        await Task.WhenAll(sendTasks);
    }

    private async Task<bool> EnqueueDevicePushAsync(string deviceToken, string title, string body, Dictionary<string, string>? data, string? imageUrl, string? clickAction, NotificationDomain domain, long? modelId = null, string? modelType = null, NotificationEventType? eventType = null, long? broadcastNotificationId = null)
    {
        if (!NotificationValidation.IsValidDeviceToken(deviceToken))
        {
            _logger.LogWarning("Invalid or empty device token. Skipping push send.");
            return false;
        }

        try
        {
            var domainStr = domain.ToStringValue();

            var pushMessage = new PushNotificationQueueMessage
            {
                Type = QueueMessageTypes.PushNotification,
                Domain = domainStr,
                DeviceToken = deviceToken,
                Title = title,
                Body = body,
                Data = data,
                ImageUrl = imageUrl,
                ClickAction = clickAction,
                ModelId = modelId,
                ModelType = modelType,
                EventType = eventType.HasValue ? (int)eventType.Value : null,
                BroadcastNotificationId = broadcastNotificationId
            };

            var queueName = _queueNameResolver.Resolve(domainStr);
            await _queueService.EnqueueAsync(queueName, pushMessage);

            _logger.LogInformation("Push notification queued to device {DeviceToken} with title: {Title} in domain: {Domain}", PiiRedactor.RedactToken(deviceToken), title, domainStr);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue push to device {DeviceToken} with title: {Title} in domain: {Domain}", PiiRedactor.RedactToken(deviceToken), title, domain.ToStringValue());
            return false;
        }
    }
}
