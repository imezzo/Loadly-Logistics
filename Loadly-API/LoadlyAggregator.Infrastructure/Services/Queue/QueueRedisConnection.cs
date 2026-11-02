using StackExchange.Redis;

namespace LoadlyAggregator.Infrastructure.Services.Queue;


public sealed class QueueRedisConnection
{
    public QueueRedisConnection(IConnectionMultiplexer multiplexer)
    {
        Multiplexer = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
    }

    public IConnectionMultiplexer Multiplexer { get; }
}
