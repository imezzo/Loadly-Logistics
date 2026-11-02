using System.Text.Json;
using Microsoft.Extensions.Logging;
namespace LoadlyAggregator.Infrastructure.Services.Queue;




public class ExampleQueueProcessor : IQueueProcessor
{
    private readonly ILogger<ExampleQueueProcessor> _logger;
    public ExampleQueueProcessor(ILogger<ExampleQueueProcessor> logger)
    {
        _logger = logger;
    }
    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing message from queue: {QueueName}", queueName);
        try
        {

            var jsonDoc = JsonDocument.Parse(messageJson);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("type", out var typeElement))
            {
                var messageType = typeElement.GetString();
                _logger.LogInformation("Message type: {MessageType}", messageType);
                switch (messageType?.ToLower())
                {
                    case "email":


                        _logger.LogInformation("Processing email message");
                        break;
                    case "notification":


                        _logger.LogInformation("Processing notification message");
                        break;
                    default:
                        _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                        break;
                }
            }
            else
            {

                _logger.LogInformation("Processing generic message");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse message JSON from queue {QueueName}", queueName);
            throw;
        }
        await Task.CompletedTask;
    }
}
