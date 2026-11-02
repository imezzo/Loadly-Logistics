using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class BroadcastNotificationEnqueueOptions : IBroadcastNotificationEnqueueOptions
{
    public BroadcastNotificationEnqueueOptions(IOptions<QueueOptions> queueOptions)
    {
        int n = queueOptions.Value.BroadcastNotificationBatchSize;
        BatchSize = n > 0 ? n : QueueProcessingDefaults.BroadcastNotificationBatchSize;
    }

    public int BatchSize { get; }
}
