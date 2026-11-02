using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Infrastructure.Options;
using LoadlyAggregator.Infrastructure.Services.Notification;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.DTOs.Queue;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class EmailQueueProcessor : IQueueProcessor
{
    private readonly ILogger<EmailQueueProcessor> _logger;
    private readonly SmtpOptions _smtpOptions;
    private readonly LogsDbContext _logsDbContext;

    public EmailQueueProcessor(
        ILogger<EmailQueueProcessor> logger,
        IOptions<SmtpOptions> smtpOptions,
        LogsDbContext logsDbContext)
    {
        _logger = logger;
        _smtpOptions = smtpOptions.Value;
        _logsDbContext = logsDbContext;
    }
    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = JsonSerializer.Deserialize<EmailQueueMessage>(
                messageJson,
                ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis);
            if (message == null)
            {
                _logger.LogError("Failed to deserialize email message from queue {QueueName}", queueName);
                return;
            }
            var emailStartTime = DateTime.UtcNow;
            _logger.LogInformation("Processing email message to {To} with subject: {Subject}", PiiRedactor.RedactEmail(message.To), message.Subject);

            var logEntry = new NotificationsLog
            {
                Channel = NotificationChannel.EMAIL,
                Recipient = message.To,
                Body = $"Subject: {message.Subject}\n\n{message.Body}",
                Status = NotificationStatus.PENDING,
                EventType = message.EventType.HasValue ? (NotificationEventType)message.EventType.Value : null,
                ModelId = message.ModelId,
                ModelType = message.ModelType,
                BroadcastNotificationId = message.BroadcastNotificationId
            };
            try
            {
                var fromEmail = message.From ?? _smtpOptions.FromEmail ?? throw new InvalidOperationException("SMTP FromEmail not configured");
                var fromName = message.FromName ?? _smtpOptions.FromName;

                using var client = new System.Net.Mail.SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
                {
                    Credentials = new System.Net.NetworkCredential(_smtpOptions.Username, _smtpOptions.Password),
                    EnableSsl = _smtpOptions.EnableSsl,
                    UseDefaultCredentials = _smtpOptions.UseDefaultCredentials
                };
                using var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(fromEmail, fromName),
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = message.IsHtml
                };
                mailMessage.To.Add(new System.Net.Mail.MailAddress(message.To, message.ToName ?? message.To));
                await client.SendMailAsync(mailMessage, cancellationToken);
                var emailDuration = (DateTime.UtcNow - emailStartTime).TotalMilliseconds;

                logEntry.Status = NotificationStatus.SUCCESS;
                logEntry.Response = JsonSerializer.Serialize(
                    new { success = true, durationMs = emailDuration },
                    ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
                _logger.LogInformation("Email sent successfully to {To} with subject: {Subject} in {Duration}ms", PiiRedactor.RedactEmail(message.To), message.Subject, emailDuration);

                _logsDbContext.NotificationsLogs.Add(logEntry);
                await _logsDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                logEntry.Status = NotificationStatus.FAILED;
                logEntry.Response = JsonSerializer.Serialize(
                    new { error = ex.Message, type = ex.GetType().Name },
                    ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
                _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", PiiRedactor.RedactEmail(message.To), message.Subject);

                try
                {
                    _logsDbContext.NotificationsLogs.Add(logEntry);
                    await _logsDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Failed to save notification log entry");
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse email message JSON from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing email message from queue {QueueName}", queueName);
        }
    }
}
