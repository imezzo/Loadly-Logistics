using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Queue;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class SmsSender : ISmsSender
{
    private readonly ILogger<SmsSender> _logger;
    private readonly IQueueService _queueService;
    private readonly IQueueNameResolver _queueNameResolver;

    public SmsSender(
        ILogger<SmsSender> logger,
        IQueueService queueService,
        IQueueNameResolver queueNameResolver)
    {
        _logger = logger;
        _queueService = queueService;
        _queueNameResolver = queueNameResolver;
    }

    public Task<bool> SendAsync(SmsMessage message, CancellationToken cancellationToken = default) =>
        EnqueueAsync(message.PhoneNumber, message.Message, message.Domain, eventType: message.EventType);

    public Task<bool> SendAsync(string phoneNumber, string message, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        EnqueueAsync(phoneNumber, message, domain, modelId, modelType, Enum.TryParse<NotificationEventType>(eventType, ignoreCase: true, out var et) ? et : null);

    private async Task<bool> EnqueueAsync(string phoneNumber, string message, NotificationDomain domain, long? modelId = null, string? modelType = null, NotificationEventType? eventType = null)
    {
        if (!NotificationValidation.IsValidPhone(phoneNumber))
        {
            _logger.LogWarning("Invalid phone number for recipient. Skipping send.");
            return false;
        }

        try
        {
            var domainStr = domain.ToStringValue();

            var smsMessage = new SmsQueueMessage
            {
                Type = QueueMessageTypes.Sms,
                Domain = domainStr,
                PhoneNumber = phoneNumber,
                Message = message,
                ModelId = modelId,
                ModelType = modelType,
                EventType = eventType.HasValue ? (int)eventType.Value : null
            };

            var queueName = _queueNameResolver.Resolve(domainStr);
            await _queueService.EnqueueAsync(queueName, smsMessage);

            _logger.LogInformation("SMS queued successfully to {PhoneNumber} in domain: {Domain}", PiiRedactor.RedactPhone(phoneNumber), domainStr);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue SMS to {PhoneNumber} in domain: {Domain}", PiiRedactor.RedactPhone(phoneNumber), domain.ToStringValue());
            return false;
        }
    }
}
