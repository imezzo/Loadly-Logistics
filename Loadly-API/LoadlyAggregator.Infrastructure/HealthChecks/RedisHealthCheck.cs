using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace LoadlyAggregator.Infrastructure.HealthChecks;

public sealed class RedisHealthCheck(IConnectionMultiplexer redis) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (!redis.IsConnected)
        {
            return HealthCheckResult.Unhealthy("Redis is not connected");
        }

        try
        {
            var latency = await redis.GetDatabase().PingAsync();
            return latency < TimeSpan.FromSeconds(5)
                ? HealthCheckResult.Healthy($"Redis ping: {latency.TotalMilliseconds:F0}ms")
                : HealthCheckResult.Degraded($"Redis ping slow: {latency.TotalMilliseconds:F0}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis ping failed", ex);
        }
    }
}
