using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;






public class RefreshTokenCleanupService : BackgroundService, IRefreshTokenCleanupService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshTokenCleanupService> _logger;
    private readonly RefreshTokenCleanupOptions _options;
    private readonly TimeProvider _timeProvider;

    public RefreshTokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<RefreshTokenCleanupService> logger,
        IOptions<RefreshTokenCleanupOptions> options,
        TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
        _timeProvider = timeProvider;


        ValidateOptions();
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    private void ValidateOptions()
    {
        if (_options.CleanupIntervalHours <= 0)
        {
            throw new InvalidOperationException(
                $"RefreshTokenCleanupIntervalHours must be greater than 0. Current value: {_options.CleanupIntervalHours}");
        }

        if (_options.ExpiredTokenRetentionDays <= 0)
        {
            throw new InvalidOperationException(
                $"ExpiredTokenRetentionDays must be greater than 0. Current value: {_options.ExpiredTokenRetentionDays}");
        }

        if (_options.RevokedTokenRetentionDays <= 0)
        {
            throw new InvalidOperationException(
                $"RevokedTokenRetentionDays must be greater than 0. Current value: {_options.RevokedTokenRetentionDays}");
        }

        if (_options.PreferredCleanupStartHour.HasValue)
        {
            if (_options.PreferredCleanupStartHour.Value < 0 || _options.PreferredCleanupStartHour.Value > 23)
            {
                throw new InvalidOperationException(
                    $"PreferredCleanupStartHour must be between 0 and 23. Current value: {_options.PreferredCleanupStartHour.Value}");
            }
        }

        if (_options.PreferredCleanupEndHour.HasValue)
        {
            if (_options.PreferredCleanupEndHour.Value < 0 || _options.PreferredCleanupEndHour.Value > 23)
            {
                throw new InvalidOperationException(
                    $"PreferredCleanupEndHour must be between 0 and 23. Current value: {_options.PreferredCleanupEndHour.Value}");
            }
        }

        if (_options.PreferredCleanupStartHour.HasValue && _options.PreferredCleanupEndHour.HasValue)
        {
            if (_options.PreferredCleanupStartHour.Value >= _options.PreferredCleanupEndHour.Value)
            {
                throw new InvalidOperationException(
                    $"PreferredCleanupStartHour ({_options.PreferredCleanupStartHour.Value}) must be less than PreferredCleanupEndHour ({_options.PreferredCleanupEndHour.Value})");
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Refresh Token Cleanup Service started. Cleanup interval: {IntervalHours} hours, " +
            "Expired retention: {ExpiredDays} days, Revoked retention: {RevokedDays} days",
            _options.CleanupIntervalHours,
            _options.ExpiredTokenRetentionDays,
            _options.RevokedTokenRetentionDays);


        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {

                var now = NowUtc;
                var currentHour = now.Hour;
                var preferredStartHour = _options.PreferredCleanupStartHour;
                var preferredEndHour = _options.PreferredCleanupEndHour;


                if (preferredStartHour.HasValue && preferredEndHour.HasValue)
                {
                    var isInPreferredWindow = currentHour >= preferredStartHour.Value &&
                                             currentHour < preferredEndHour.Value;

                    if (!isInPreferredWindow)
                    {

                        DateTime nextWindowStart;
                        var todayWindowStart = new DateTime(now.Year, now.Month, now.Day, preferredStartHour.Value, 0, 0, DateTimeKind.Utc);

                        if (now < todayWindowStart)
                        {

                            nextWindowStart = todayWindowStart;
                        }
                        else
                        {

                            nextWindowStart = todayWindowStart.AddDays(1);
                        }

                        var timeUntilWindow = nextWindowStart - now;

                        _logger.LogInformation(
                            "Skipping cleanup - outside preferred window ({StartHour:00}:00-{EndHour:00}:00 UTC). " +
                            "Current time: {CurrentTime:yyyy-MM-dd HH:mm} UTC. " +
                            "Waiting {Hours} hours {Minutes} minutes until next window starts at {NextWindowStart:yyyy-MM-dd HH:mm} UTC",
                            preferredStartHour.Value,
                            preferredEndHour.Value,
                            now,
                            (int)timeUntilWindow.TotalHours,
                            timeUntilWindow.Minutes,
                            nextWindowStart);



                        var totalMinutes = (int)timeUntilWindow.TotalMinutes;
                        var checkIntervalMinutes = Math.Min(60, totalMinutes);

                        for (var remainingMinutes = totalMinutes; remainingMinutes > 0; remainingMinutes -= checkIntervalMinutes)
                        {
                            var waitMinutes = Math.Min(checkIntervalMinutes, remainingMinutes);
                            await Task.Delay(TimeSpan.FromMinutes(waitMinutes), stoppingToken);
                        }

                        continue;
                    }
                }

                var result = await PerformCleanupAsync(stoppingToken);
                if (!result.Success)
                {
                    _logger.LogWarning("Cleanup completed with errors: {Error}", result.ErrorMessage);
                }



                if (preferredStartHour.HasValue && preferredEndHour.HasValue)
                {

                    var nextCleanupTime = NowUtc.AddHours(_options.CleanupIntervalHours);
                    var nextCleanupHour = nextCleanupTime.Hour;


                    var willBeInWindow = nextCleanupHour >= preferredStartHour.Value &&
                                        nextCleanupHour < preferredEndHour.Value;

                    if (!willBeInWindow)
                    {

                        var nextWindowStart = new DateTime(
                            nextCleanupTime.Year,
                            nextCleanupTime.Month,
                            nextCleanupTime.Day,
                            preferredStartHour.Value,
                            0,
                            0,
                            DateTimeKind.Utc);


                        if (nextCleanupTime > nextWindowStart.AddHours(preferredEndHour.Value - preferredStartHour.Value))
                        {
                            nextWindowStart = nextWindowStart.AddDays(1);
                        }
                        else if (nextCleanupTime > nextWindowStart)
                        {


                            if (nextCleanupHour >= preferredEndHour.Value)
                            {
                                nextWindowStart = nextWindowStart.AddDays(1);
                            }
                        }

                        var waitUntil = nextWindowStart - NowUtc;
                        _logger.LogInformation(
                            "Next scheduled cleanup at {NextCleanupTime:yyyy-MM-dd HH:mm} UTC is outside preferred window. " +
                            "Adjusting to next window start at {NextWindowStart:yyyy-MM-dd HH:mm} UTC. " +
                            "Waiting {Hours} hours {Minutes} minutes",
                            nextCleanupTime,
                            nextWindowStart,
                            (int)waitUntil.TotalHours,
                            waitUntil.Minutes);

                        await Task.Delay(waitUntil, stoppingToken);
                    }
                    else
                    {

                        await Task.Delay(TimeSpan.FromHours(_options.CleanupIntervalHours), stoppingToken);
                    }
                }
                else
                {

                    await Task.Delay(TimeSpan.FromHours(_options.CleanupIntervalHours), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Refresh Token Cleanup Service is being cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during refresh token cleanup");

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Refresh Token Cleanup Service stopped");
    }




    public async Task<RefreshTokenCleanupResult> PerformCleanupAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting refresh token cleanup (manual trigger)");

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

        var startTime = NowUtc;
        var result = new RefreshTokenCleanupResult();

        try
        {

            var expiredDeleted = await repository.DeleteExpiredTokensAsync(
                olderThanDays: _options.ExpiredTokenRetentionDays,
                batchSize: _options.BatchSize,
                cancellationToken: cancellationToken);
            result.ExpiredTokensDeleted = expiredDeleted;

            _logger.LogInformation(
                "Deleted {Count} expired refresh tokens (older than {Days} days)",
                expiredDeleted,
                _options.ExpiredTokenRetentionDays);


            var revokedDeleted = await repository.DeleteOldRevokedTokensAsync(
                olderThanDays: _options.RevokedTokenRetentionDays,
                batchSize: _options.BatchSize,
                cancellationToken: cancellationToken);
            result.RevokedTokensDeleted = revokedDeleted;

            _logger.LogInformation(
                "Deleted {Count} old revoked refresh tokens (older than {Days} days)",
                revokedDeleted,
                _options.RevokedTokenRetentionDays);

            result.DurationMs = (long)(NowUtc - startTime).TotalMilliseconds;
            result.Success = true;

            _logger.LogInformation(
                "Refresh token cleanup completed. Total deleted: {Total}, Duration: {Duration}ms",
                result.TotalDeleted,
                result.DurationMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token cleanup");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.DurationMs = (long)(NowUtc - startTime).TotalMilliseconds;
        }

        return result;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh Token Cleanup Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}




public class RefreshTokenCleanupOptions
{



    public int CleanupIntervalHours { get; set; } = 24;




    public int ExpiredTokenRetentionDays { get; set; } = 7;




    public int RevokedTokenRetentionDays { get; set; } = 30;





    public int BatchSize { get; set; } = 1000;





    public int? PreferredCleanupStartHour { get; set; }





    public int? PreferredCleanupEndHour { get; set; }
}

