namespace LoadlyAggregator.Domain.Common.Interfaces;



public interface IQueueService
{







    Task EnqueueAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;







    Task<T?> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default) where T : class;






    Task<long> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default);





    Task ClearQueueAsync(string queueName, CancellationToken cancellationToken = default);






    Task<string?> DequeueStringAsync(string queueName, CancellationToken cancellationToken = default);
}
