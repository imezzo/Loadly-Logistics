namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IBroadcastNotificationEnqueueOptions
{
    int BatchSize { get; }
}
