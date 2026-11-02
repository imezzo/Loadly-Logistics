namespace LoadlyAggregator.Shared.DTOs.Queue;

public abstract class QueueMessageBase
{
    public string Type { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public long? ModelId { get; set; }
    public string? ModelType { get; set; }
    public int? EventType { get; set; }
    public long? BroadcastNotificationId { get; set; }
    public int RetryCount { get; set; } = 0;
}
