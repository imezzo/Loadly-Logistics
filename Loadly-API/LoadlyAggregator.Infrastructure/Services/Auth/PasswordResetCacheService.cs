using LoadlyAggregator.Shared.Enums;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;






public class PasswordResetCacheService : IPasswordResetCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly int _maxFailedAttempts;
    private readonly int _rateLimitWindowMinutes;
    private readonly int _passwordResetCodeMinutes;

    public PasswordResetCacheService(IDistributedCache cache, IConfiguration configuration, TimeProvider timeProvider)
    {
        _cache = cache;
        _configuration = configuration;
        _timeProvider = timeProvider;
        _maxFailedAttempts = int.Parse(configuration["AuthSettings:MaxFailedAttempts"] ?? "5");
        _rateLimitWindowMinutes = int.Parse(configuration["AuthSettings:RateLimitMinutes"] ?? "60");
        _passwordResetCodeMinutes = int.Parse(configuration["AuthSettings:PasswordResetCodeMinutes"] ?? "60");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task StoreResetAttemptAsync(
        string userType,
        string identifier,
        long userId,
        string code,
        CommunicationChannel channel,
        int expirationMinutes = 0)
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


        var expiration = expirationMinutes > 0 ? expirationMinutes : _passwordResetCodeMinutes;

        var json = JsonSerializer.Serialize(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration)
        };

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<PasswordResetAttempt?> GetResetAttemptAsync(string userType, string identifier)
    {
        var key = GetResetKey(userType, identifier);
        var json = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<PasswordResetAttempt>(json);
    }

    public async Task DeleteResetAttemptAsync(string userType, string identifier)
    {
        var resetKey = GetResetKey(userType, identifier);
        var attemptsKey = GetAttemptsKey(userType, identifier);

        await _cache.RemoveAsync(resetKey);
        await _cache.RemoveAsync(attemptsKey);
    }

    public async Task IncrementFailedAttemptsAsync(string userType, string identifier)
    {

        var resetAttempt = await GetResetAttemptAsync(userType, identifier);
        if (resetAttempt != null)
        {
            resetAttempt.FailedAttempts++;
            var key = GetResetKey(userType, identifier);
            var json = JsonSerializer.Serialize(resetAttempt);


            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_passwordResetCodeMinutes)
            };

            await _cache.SetStringAsync(key, json, options);
        }


        var attemptsKey = GetAttemptsKey(userType, identifier);
        var attemptsJson = await _cache.GetStringAsync(attemptsKey);

        int currentAttempts = 0;
        if (!string.IsNullOrEmpty(attemptsJson))
        {
            currentAttempts = JsonSerializer.Deserialize<int>(attemptsJson);
        }

        currentAttempts++;
        var newAttemptsJson = JsonSerializer.Serialize(currentAttempts);
        var rateLimitOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_rateLimitWindowMinutes)
        };

        await _cache.SetStringAsync(attemptsKey, newAttemptsJson, rateLimitOptions);
    }

    public async Task<bool> IsRateLimitedAsync(string userType, string identifier)
    {

        var attemptsKey = GetAttemptsKey(userType, identifier);
        var attemptsJson = await _cache.GetStringAsync(attemptsKey);

        if (string.IsNullOrEmpty(attemptsJson))
            return false;

        var attempts = JsonSerializer.Deserialize<int>(attemptsJson);
        return attempts >= _maxFailedAttempts;
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

