using System.Text.Json;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Infrastructure.Services.Notification;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.DTOs.Queue;
namespace LoadlyAggregator.Infrastructure.Services.Queue;



public class SmsQueueProcessor : IQueueProcessor
{
    private readonly ILogger<SmsQueueProcessor> _logger;
    private readonly ISmsProvider _smsProvider;
    private readonly LogsDbContext _logsDbContext;
    public SmsQueueProcessor(
        ILogger<SmsQueueProcessor> logger,
        ISmsProvider smsProvider,
        LogsDbContext logsDbContext)
    {
        _logger = logger;
        _smsProvider = smsProvider;
        _logsDbContext = logsDbContext;
    }
    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = JsonSerializer.Deserialize<SmsQueueMessage>(
                messageJson,
                ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis);
            if (message == null)
            {
                _logger.LogError("Failed to deserialize SMS message from queue {QueueName}", queueName);
                return;
            }
            _logger.LogInformation("Processing SMS message to {PhoneNumber}", PiiRedactor.RedactPhone(message.PhoneNumber));

            var logEntry = new NotificationsLog
            {
                Channel = NotificationChannel.SMS,
                Recipient = message.PhoneNumber,
                Body = message.Message,
                Status = NotificationStatus.PENDING,
                EventType = message.EventType.HasValue ? (NotificationEventType)message.EventType.Value : null,
                ModelId = message.ModelId,
                ModelType = message.ModelType,
                BroadcastNotificationId = message.BroadcastNotificationId
            };
            try
            {
                string? responseJson = null;
                var result = await _smsProvider.SendSmsAsync(message.PhoneNumber, message.Message, r => responseJson = r);

                logEntry.Status = result ? NotificationStatus.SUCCESS : NotificationStatus.FAILED;
                logEntry.Response = responseJson;
                if (result)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", PiiRedactor.RedactPhone(message.PhoneNumber));
                }
                else
                {
                    _logger.LogWarning("Failed to send SMS to {PhoneNumber}", PiiRedactor.RedactPhone(message.PhoneNumber));
                }

                _logsDbContext.NotificationsLogs.Add(logEntry);
                await _logsDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logEntry.Status = NotificationStatus.FAILED;
                logEntry.Response = JsonSerializer.Serialize(
                    new { error = ex.Message, type = ex.GetType().Name },
                    ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
                _logger.LogError(ex, "Exception occurred while sending SMS to {PhoneNumber}", PiiRedactor.RedactPhone(message.PhoneNumber));

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
            _logger.LogError(ex, "Failed to parse SMS message JSON from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SMS message from queue {QueueName}", queueName);
        }
    }
}
