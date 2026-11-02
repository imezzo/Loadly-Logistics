using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public interface IQueueProcessorResolver
{
    (IQueueProcessor? Processor, string? MessageType) Resolve(string messageJson, string queueName);
}

public class QueueProcessorResolver : IQueueProcessorResolver
{
    private static readonly Dictionary<string, string> QueueNameToTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["emails"] = QueueMessageTypes.Email,
        ["sms"] = QueueMessageTypes.Sms,
        ["push_notifications"] = QueueMessageTypes.PushNotification,
        ["push"] = QueueMessageTypes.PushNotification
    };

    private readonly ILogger<QueueProcessorResolver> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<string, Type> _processorTypes;

    public QueueProcessorResolver(
        ILogger<QueueProcessorResolver> logger,
        IServiceProvider serviceProvider,
        IOptions<QueueProcessorRegistryOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _processorTypes = options.Value.ProcessorMappings ?? new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }

    public (IQueueProcessor? Processor, string? MessageType) Resolve(string messageJson, string queueName)
    {
        var messageType = ResolveMessageType(messageJson, queueName);
        if (messageType == null)
        {
            return (null, null);
        }

        if (_processorTypes.TryGetValue(messageType, out var processorType))
        {
            try
            {
                var processor = (IQueueProcessor)_serviceProvider.GetRequiredService(processorType);
                return (processor, messageType);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve processor for type {MessageType}", messageType);
                return (null, messageType);
            }
        }

        return (null, messageType);
    }

    private string? ResolveMessageType(string messageJson, string queueName)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(messageJson);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("type", out var typeElement))
            {
                var type = typeElement.GetString()?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(type))
                {
                    return type;
                }
            }

            if (root.TryGetProperty("domain", out _))
            {
                return InferTypeFromStructure(root);
            }

            return QueueNameToTypeMap.TryGetValue(queueName, out var mappedType) ? mappedType : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? InferTypeFromStructure(JsonElement root)
    {
        if (root.TryGetProperty("phoneNumber", out _) || root.TryGetProperty("phone_number", out _))
        {
            return QueueMessageTypes.Sms;
        }

        if (root.TryGetProperty("to", out _) || root.TryGetProperty("subject", out _))
        {
            return QueueMessageTypes.Email;
        }

        if (root.TryGetProperty("deviceToken", out _) || root.TryGetProperty("device_token", out _))
        {
            return QueueMessageTypes.PushNotification;
        }

        return null;
    }
}
