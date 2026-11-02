using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class CompositeQueueProcessor : IQueueProcessor
{
    private readonly ILogger<CompositeQueueProcessor> _logger;
    private readonly IQueueProcessorResolver _resolver;

    public CompositeQueueProcessor(
        ILogger<CompositeQueueProcessor> logger,
        IQueueProcessorResolver resolver)
    {
        _logger = logger;
        _resolver = resolver;
    }

    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var (processor, messageType) = _resolver.Resolve(messageJson, queueName);
            if (processor != null)
            {
                await processor.ProcessMessageAsync(messageJson, queueName, cancellationToken);
                return;
            }

            _logger.LogWarning(
                messageType != null
                    ? "Unknown message type: {MessageType} in queue {QueueName}"
                    : "No message type found and queue name '{QueueName}' is not recognized",
                messageType,
                queueName);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse message JSON from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
        }
    }
}
