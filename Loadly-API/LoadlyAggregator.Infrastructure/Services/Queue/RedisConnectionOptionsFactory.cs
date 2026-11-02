using StackExchange.Redis;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

internal static class RedisConnectionOptionsFactory
{
    public static ConfigurationOptions Create(string connectionString)
    {
        var options = ConfigurationOptions.Parse(connectionString);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 10_000;
        options.SyncTimeout = 10_000;
        options.AsyncTimeout = 10_000;
        options.KeepAlive = 60;
        options.ReconnectRetryPolicy = new ExponentialRetry(5_000);
        return options;
    }
}
