namespace LoadlyAggregator.Infrastructure.Services.Queue;

/// <summary>
/// Configuration for queue processor type mapping. Add new processors without modifying QueueProcessorResolver (OCP).
/// </summary>
public class QueueProcessorRegistryOptions
{
    public Dictionary<string, Type> ProcessorMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
