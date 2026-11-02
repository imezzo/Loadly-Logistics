using LoadlyAggregator.Shared.Enums;
using System.Text.Json;
using StackExchange.Redis;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Auth;




public class RedisPasswordResetCacheService : IPasswordResetCacheService
{
    private readonly IDatabase _redis;
    private readonly TimeProvider _timeProvider;
    private const int MaxFailedAttempts = 5;
    private const int RateLimitWindowMinutes = 60;
    public RedisPasswordResetCacheService(IConnectionMultiplexer connectionMultiplexer, TimeProvider timeProvider)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _timeProvider = timeProvider;
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task StoreResetAttemptAsync(
        string userType,
        string identifier,
        long userId,
        string code,
        CommunicationChannel channel,
        int expirationMinutes = 60)
    {
        var key = GetResetKey(userType, identifier);
        var data = new PasswordResetAttempt
        {
            UserId = userId,
            Code = code,
            Channel = channel,
            CreatedAt = NowUtc,
            FailedAttempts = 0
        };
        var json = JsonSerializer.Serialize(data);
        await _redis.StringSetAsync(key, json, TimeSpan.FromMinutes(expirationMinutes));
    }
    public async Task<PasswordResetAttempt?> GetResetAttemptAsync(string userType, string identifier)
    {
        var key = GetResetKey(userType, identifier);
        var json = await _redis.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return null;
        return JsonSerializer.Deserialize<PasswordResetAttempt>(json.ToString());
    }
    public async Task DeleteResetAttemptAsync(string userType, string identifier)
    {
        var resetKey = GetResetKey(userType, identifier);
        var attemptsKey = GetAttemptsKey(userType, identifier);
        await Task.WhenAll(
            _redis.KeyDeleteAsync(resetKey),
            _redis.KeyDeleteAsync(attemptsKey)
        );
    }
    public async Task IncrementFailedAttemptsAsync(string userType, string identifier)
    {

        var resetAttempt = await GetResetAttemptAsync(userType, identifier);
        if (resetAttempt != null)
        {
            resetAttempt.FailedAttempts++;
            var key = GetResetKey(userType, identifier);
            var json = JsonSerializer.Serialize(resetAttempt);
            await _redis.StringSetAsync(key, json, TimeSpan.FromMinutes(60));
        }

        var attemptsKey = GetAttemptsKey(userType, identifier);
        await _redis.StringIncrementAsync(attemptsKey);
        await _redis.KeyExpireAsync(attemptsKey, TimeSpan.FromMinutes(RateLimitWindowMinutes));
    }
    public async Task<bool> IsRateLimitedAsync(string userType, string identifier)
    {
        var attemptsKey = GetAttemptsKey(userType, identifier);
        var attempts = await _redis.StringGetAsync(attemptsKey);
        if (attempts.IsNullOrEmpty)
            return false;
        return (int)attempts >= MaxFailedAttempts;
    }
    private static string GetResetKey(string userType, string identifier)
    {
        return $"pwd_reset:{userType}:{identifier.ToLowerInvariant()}";
    }
    private static string GetAttemptsKey(string userType, string identifier)
    {
        return $"pwd_reset_attempts:{userType}:{identifier.ToLowerInvariant()}";
    }
}
