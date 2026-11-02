namespace LoadlyAggregator.Infrastructure.Services.Queue;



public interface IQueueProcessor
{






    Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default);
}
