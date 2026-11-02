using LoadlyAggregator.Shared.Enums;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;






public class MfaCacheService : IMfaCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly int _maxFailedAttempts;
    private readonly int _rateLimitWindowMinutes;
    private readonly int _mfaCodeMinutes;

    public int MfaCodeExpirationMinutes => _mfaCodeMinutes;

    public MfaCacheService(IDistributedCache cache, IConfiguration configuration, TimeProvider timeProvider)
    {
        _cache = cache;
        _configuration = configuration;
        _timeProvider = timeProvider;
        _maxFailedAttempts = int.Parse(configuration["AuthSettings:MaxFailedAttempts"] ?? "5");
        _rateLimitWindowMinutes = int.Parse(configuration["AuthSettings:RateLimitMinutes"] ?? "60");
        _mfaCodeMinutes = int.Parse(configuration["AuthSettings:MfaCodeMinutes"] ?? "30");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task StoreMfaCodeAsync(
        long userId,
        string userType,
        string code,
        CommunicationChannel channel,
        int expirationMinutes = 0)
    {
        var key = GetMfaKey(userType, userId);
        var data = new MfaCodeAttempt
        {
            UserId = userId,
            UserType = userType,
            Code = code,
            Channel = channel,
            CreatedAt = NowUtc,
            FailedAttempts = 0
        };


        var expiration = expirationMinutes > 0 ? expirationMinutes : _mfaCodeMinutes;

        var json = JsonSerializer.Serialize(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration)
        };

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<MfaCodeAttempt?> GetMfaCodeAsync(long userId, string userType)
    {
        var key = GetMfaKey(userType, userId);
        var json = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<MfaCodeAttempt>(json);
    }

    public async Task DeleteMfaCodeAsync(long userId, string userType)
    {
        var codeKey = GetMfaKey(userType, userId);
        var attemptsKey = GetAttemptsKey(userType, userId);

        await _cache.RemoveAsync(codeKey);
        await _cache.RemoveAsync(attemptsKey);
    }

    public async Task IncrementFailedAttemptsAsync(long userId, string userType)
    {

        var mfaAttempt = await GetMfaCodeAsync(userId, userType);
        if (mfaAttempt != null)
        {
            mfaAttempt.FailedAttempts++;
            var key = GetMfaKey(userType, userId);
            var json = JsonSerializer.Serialize(mfaAttempt);


            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_mfaCodeMinutes)
            };

            await _cache.SetStringAsync(key, json, options);
        }


        var attemptsKey = GetAttemptsKey(userType, userId);
        var attemptsJson = await _cache.GetStringAsync(attemptsKey);

        int currentAttempts = 0;
        if (!string.IsNullOrEmpty(attemptsJson))
        {
            currentAttempts = JsonSerializer.Deserialize<int>(attemptsJson);
        }

        currentAttempts++;

        var attemptsOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_rateLimitWindowMinutes)
        };

        await _cache.SetStringAsync(
            attemptsKey,
            JsonSerializer.Serialize(currentAttempts),
            attemptsOptions);
    }

    public async Task<bool> IsRateLimitedAsync(long userId, string userType)
    {
        var key = GetAttemptsKey(userType, userId);
        var json = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(json))
            return false;

        var attempts = JsonSerializer.Deserialize<int>(json);
        return attempts >= _maxFailedAttempts;
    }

    private static string GetMfaKey(string userType, long userId)
        => $"mfa_code:{userType}:{userId}";

    private static string GetAttemptsKey(string userType, long userId)
        => $"mfa_attempts:{userType}:{userId}";

    public async Task StoreLoginOtpTokenAsync(
        long userId,
        string userType,
        string token,
        int expirationMinutes = 0)
    {
        var key = GetLoginOtpTokenKey(userType, userId);
        var expiration = expirationMinutes > 0 ? expirationMinutes : _mfaCodeMinutes;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration)
        };

        await _cache.SetStringAsync(key, token, options);
    }

    public async Task<string?> GetLoginOtpTokenAsync(long userId, string userType)
    {
        var key = GetLoginOtpTokenKey(userType, userId);
        return await _cache.GetStringAsync(key);
    }

    public async Task DeleteLoginOtpTokenAsync(long userId, string userType)
    {
        var key = GetLoginOtpTokenKey(userType, userId);
        await _cache.RemoveAsync(key);
    }

    private static string GetLoginOtpTokenKey(string userType, long userId)
        => $"login_otp_token:{userType}:{userId}";
}

