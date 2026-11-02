using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Infrastructure.Options;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Queue;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;
    private readonly ILogger<EmailSender> _logger;
    private readonly IQueueService _queueService;
    private readonly IQueueNameResolver _queueNameResolver;

    public EmailSender(
        IOptions<SmtpOptions> smtpOptions,
        ILogger<EmailSender> logger,
        IQueueService queueService,
        IQueueNameResolver queueNameResolver)
    {
        _smtpOptions = smtpOptions.Value;
        _logger = logger;
        _queueService = queueService;
        _queueNameResolver = queueNameResolver;
    }

    public Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        string from = message.From ?? _smtpOptions.FromEmail ?? throw new InvalidOperationException("SMTP FromEmail not configured");
        string fromName = message.FromName ?? _smtpOptions.FromName;

        return EnqueueAsync(from, fromName, message.To, message.ToName, message.Subject, message.Body, message.IsHtml, message.Domain, eventType: message.EventType);
    }

    public Task<bool> SendAsync(string from, string fromName, string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        EnqueueAsync(from, fromName, to, toName, subject, body, isHtml, domain, modelId, modelType, Enum.TryParse<NotificationEventType>(eventType, ignoreCase: true, out var et) ? et : null);

    private async Task<bool> EnqueueAsync(string from, string fromName, string to, string toName, string subject, string body, bool isHtml, NotificationDomain domain, long? modelId = null, string? modelType = null, NotificationEventType? eventType = null)
    {
        if (!NotificationValidation.IsValidEmail(to))
        {
            _logger.LogWarning("Invalid email address for recipient. Skipping send.");
            return false;
        }

        try
        {
            var domainStr = domain.ToStringValue();

            var emailMessage = new EmailQueueMessage
            {
                Type = QueueMessageTypes.Email,
                Domain = domainStr,
                From = from,
                FromName = fromName,
                To = to,
                ToName = toName,
                Subject = subject,
                Body = body,
                IsHtml = isHtml,
                ModelId = modelId,
                ModelType = modelType,
                EventType = eventType.HasValue ? (int)eventType.Value : null
            };

            var queueName = _queueNameResolver.Resolve(domainStr);
            await _queueService.EnqueueAsync(queueName, emailMessage);

            _logger.LogInformation("Email queued successfully to {To} with subject: {Subject} in domain: {Domain}", PiiRedactor.RedactEmail(to), subject, domainStr);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue email to {To} with subject: {Subject} in domain: {Domain}", PiiRedactor.RedactEmail(to), subject, domain.ToStringValue());
            return false;
        }
    }
}
