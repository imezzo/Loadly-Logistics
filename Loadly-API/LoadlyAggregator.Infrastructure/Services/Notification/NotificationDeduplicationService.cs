using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Infrastructure.Options;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class NotificationDeduplicationService : INotificationDeduplicationService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly NotificationDeduplicationOptions _options;
    private readonly ILogger<NotificationDeduplicationService> _logger;

    public NotificationDeduplicationService(
        IConnectionMultiplexer redis,
        IOptions<NotificationDeduplicationOptions> options,
        ILogger<NotificationDeduplicationService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _options = options?.Value ?? new NotificationDeduplicationOptions();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> TryAcquireAsync(
        NotificationChannel channel,
        long modelId,
        string modelType,
        NotificationEventType eventType,
        string recipient,
        long? resourceId = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(recipient))
        {
            return true;
        }

        var key = BuildKey(channel, modelId, modelType, eventType, recipient, resourceId);
        var ttl = TimeSpan.FromMinutes(_options.WindowMinutes);

        try
        {
            var db = _redis.GetDatabase();
            var redisKey = (RedisKey)key;
            var acquired = await db.StringSetAsync(redisKey, "1", ttl, when: When.NotExists);

            if (!acquired)
            {
                _logger.LogDebug(
                    "Notification deduplication: skipping duplicate for {Channel} {EventType} resource {ResourceId} (model {ModelType}:{ModelId})",
                    channel,
                    eventType,
                    resourceId,
                    modelType,
                    modelId);
            }

            return acquired;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Deduplication check failed for key {Key}. Allowing send.", key);
            return true;
        }
    }

    private static string BuildKey(
        NotificationChannel channel,
        long modelId,
        string modelType,
        NotificationEventType eventType,
        string recipient,
        long? resourceId)
    {
        var resourcePart = resourceId.HasValue ? $":{resourceId.Value}" : "";
        var payload = $"{(int)channel}:{modelId}:{modelType}:{eventType}{resourcePart}:{recipient}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        return $"notif_dedup:{hash}";
    }
}
