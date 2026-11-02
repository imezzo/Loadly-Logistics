using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Queue;




public class RedisQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RedisQueueBackgroundService> _logger;
    private readonly QueueOptions _options;

    public RedisQueueBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RedisQueueBackgroundService> logger,
        IOptions<QueueOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Redis Queue Background Service is starting");


        var tasks = _options.Queues.Select(queueName =>
            ProcessQueueAsync(queueName, stoppingToken)
        ).ToArray();

        await Task.WhenAll(tasks);
    }

    private async Task ProcessQueueAsync(string queueName, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to process queue: {QueueName}", queueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
                var processor = scope.ServiceProvider.GetService<IQueueProcessor>();


                var messageJsonString = await queueService.DequeueStringAsync(queueName, stoppingToken);

                if (!string.IsNullOrEmpty(messageJsonString))
                {
                    var processingStartTime = DateTime.UtcNow;

                    if (processor != null)
                    {
                        await processor.ProcessMessageAsync(messageJsonString, queueName, stoppingToken);

                        var processingDuration = (DateTime.UtcNow - processingStartTime).TotalMilliseconds;
                        _logger.LogInformation("Message processed from queue {QueueName} in {Duration}ms", queueName, processingDuration);
                    }
                    else
                    {
                        _logger.LogWarning("No IQueueProcessor registered. Message will be logged only.");
                        _logger.LogInformation("Message from queue {QueueName}: {Message}", queueName, messageJsonString);
                    }
                }
                else
                {

                    await Task.Delay(_options.PollingIntervalMs, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Queue processing for {QueueName} is being cancelled", queueName);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queue {QueueName}", queueName);

                await Task.Delay(_options.ErrorRetryDelayMs, stoppingToken);
            }
        }

        _logger.LogInformation("Stopped processing queue: {QueueName}", queueName);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Redis Queue Background Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}




public class QueueOptions
{
    public List<string> Queues { get; set; } = new();

    public Dictionary<string, string>? DomainToQueueMapping { get; set; }

    public string? DefaultQueue { get; set; }

    public int PollingIntervalMs { get; set; } = 1000;

    public int ErrorRetryDelayMs { get; set; } = 5000;

    public int BroadcastNotificationBatchSize { get; set; } = 200;
}

