using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Infrastructure.Services.Queue;




public class RedisQueueService : IQueueService
{
    private readonly QueueRedisConnection _queueRedis;
    private readonly ILogger<RedisQueueService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisQueueService(QueueRedisConnection queueRedis, ILogger<RedisQueueService> logger)
    {
        _queueRedis = queueRedis ?? throw new ArgumentNullException(nameof(queueRedis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // CreateForQueueAndRedis applies ApiDateTimeJsonSerializerOptions.ApplyTo (UTC DateTime in JSON).
        _jsonOptions = ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis;
    }

    public async Task EnqueueAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var db = _queueRedis.Multiplexer.GetDatabase();
            var queueKey = GetQueueKey(queueName);
            var serializedMessage = JsonSerializer.Serialize(message, _jsonOptions);

            var enqueueTime = DateTime.UtcNow;
            await db.ListRightPushAsync(queueKey, serializedMessage);

            _logger.LogInformation("Message enqueued to queue '{QueueName}' at {Timestamp}", queueName, enqueueTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enqueueing message to queue '{QueueName}'", queueName);
            throw;
        }
    }

    public async Task<T?> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            var db = _queueRedis.Multiplexer.GetDatabase();
            var queueKey = GetQueueKey(queueName);

            var serializedMessage = await db.ListLeftPopAsync(queueKey);

            if (serializedMessage.IsNullOrEmpty)
            {
                return null;
            }

            var message = JsonSerializer.Deserialize<T>(serializedMessage.ToString(), _jsonOptions);
            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dequeuing message from queue '{QueueName}'", queueName);
            throw;
        }
    }

    public async Task<long> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            var db = _queueRedis.Multiplexer.GetDatabase();
            var queueKey = GetQueueKey(queueName);

            return await db.ListLengthAsync(queueKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue length for '{QueueName}'", queueName);
            throw;
        }
    }

    public async Task ClearQueueAsync(string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            var db = _queueRedis.Multiplexer.GetDatabase();
            var queueKey = GetQueueKey(queueName);

            await db.KeyDeleteAsync(queueKey);
            _logger.LogInformation("Queue '{QueueName}' cleared", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing queue '{QueueName}'", queueName);
            throw;
        }
    }

    public async Task<string?> DequeueStringAsync(string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            var db = _queueRedis.Multiplexer.GetDatabase();
            var queueKey = GetQueueKey(queueName);

            var dequeueStartTime = DateTime.UtcNow;
            var serializedMessage = await db.ListLeftPopAsync(queueKey);

            if (serializedMessage.IsNullOrEmpty)
            {
                return null;
            }

            var dequeueDuration = (DateTime.UtcNow - dequeueStartTime).TotalMilliseconds;
            _logger.LogInformation("Message dequeued from queue '{QueueName}' in {Duration}ms", queueName, dequeueDuration);

            return serializedMessage!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dequeuing message from queue '{QueueName}'", queueName);
            throw;
        }
    }

    private static string GetQueueKey(string queueName)
    {
        return $"queue:{queueName}";
    }
}

