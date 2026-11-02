using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.DTOs.Queue;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class RetryableQueueProcessor : IQueueProcessor
{
    private readonly IQueueProcessor _innerProcessor;
    private readonly IQueueService _queueService;
    private readonly ILogger<RetryableQueueProcessor> _logger;
    private readonly int _maxRetries;
    private readonly int _baseDelayMs;

    public RetryableQueueProcessor(
        IQueueProcessor innerProcessor,
        IQueueService queueService,
        ILogger<RetryableQueueProcessor> logger,
        IConfiguration configuration)
    {
        _innerProcessor = innerProcessor;
        _queueService = queueService;
        _logger = logger;
        _maxRetries = int.Parse(configuration["Queue:MaxRetries"] ?? "3");
        _baseDelayMs = int.Parse(configuration["Queue:BaseRetryDelayMs"] ?? "1000");
    }

    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _innerProcessor.ProcessMessageAsync(messageJson, queueName, cancellationToken);
        }
        catch (Exception ex)
        {
            await HandleFailureWithRetryAsync(messageJson, queueName, ex, cancellationToken);
        }
    }

    private async Task HandleFailureWithRetryAsync(string messageJson, string queueName, Exception ex, CancellationToken cancellationToken)
    {
        try
        {
            var messageBase = JsonSerializer.Deserialize<QueueMessageBase>(
                messageJson,
                ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis);

            if (messageBase == null)
            {
                _logger.LogError(ex, "Failed to deserialize message for retry logic from queue {QueueName}", queueName);
                return;
            }

            var currentRetryCount = messageBase.RetryCount;

            if (currentRetryCount >= _maxRetries)
            {
                _logger.LogError(ex, "Message failed after {MaxRetries} retries. Moving to dead-letter queue. Queue: {QueueName}", _maxRetries, queueName);
                await MoveToDeadLetterQueueAsync(messageJson, queueName, ex, cancellationToken);
                return;
            }

            var nextRetryCount = currentRetryCount + 1;
            var delayMs = CalculateExponentialBackoffDelay(nextRetryCount);

            _logger.LogWarning(ex, "Message processing failed. Retry {RetryCount}/{MaxRetries} scheduled in {DelayMs}ms. Queue: {QueueName}",
                nextRetryCount, _maxRetries, delayMs, queueName);

            await Task.Delay(delayMs, cancellationToken);

            var updatedMessage = UpdateRetryCount(messageJson, nextRetryCount);
            await _queueService.EnqueueAsync(queueName, updatedMessage);
        }
        catch (Exception retryEx)
        {
            _logger.LogError(retryEx, "Failed to handle retry logic for message in queue {QueueName}. Original exception: {OriginalException}",
                queueName, ex.Message);
        }
    }

    private int CalculateExponentialBackoffDelay(int retryCount)
    {
        return _baseDelayMs * (int)Math.Pow(2, retryCount - 1);
    }

    private string UpdateRetryCount(string messageJson, int newRetryCount)
    {
        var jsonDoc = JsonDocument.Parse(messageJson);
        var root = jsonDoc.RootElement;

        var properties = new Dictionary<string, object?>();
        foreach (var property in root.EnumerateObject())
        {
            if (property.Name.Equals("RetryCount", StringComparison.OrdinalIgnoreCase))
            {
                properties[property.Name] = newRetryCount;
            }
            else
            {
                properties[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.Number => property.Value.GetInt64(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, string>>(
                        property.Value.GetRawText(),
                        ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis),
                    _ => null
                };
            }
        }

        if (!properties.ContainsKey("RetryCount"))
        {
            properties["RetryCount"] = newRetryCount;
        }

        return JsonSerializer.Serialize(properties, ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
    }

    private async Task MoveToDeadLetterQueueAsync(string messageJson, string queueName, Exception ex, CancellationToken cancellationToken)
    {
        try
        {
            var dlqName = $"{queueName}:dlq";
            var dlqMessage = new
            {
                OriginalQueue = queueName,
                OriginalMessage = messageJson,
                FailureReason = ex.Message,
                ExceptionType = ex.GetType().Name,
                StackTrace = ex.StackTrace,
                FailedAt = DateTime.UtcNow
            };

            await _queueService.EnqueueAsync(dlqName, dlqMessage);
            _logger.LogInformation("Message moved to dead-letter queue: {DlqName}", dlqName);
        }
        catch (Exception dlqEx)
        {
            _logger.LogError(dlqEx, "Failed to move message to dead-letter queue {QueueName}:dlq", queueName);
        }
    }
}
