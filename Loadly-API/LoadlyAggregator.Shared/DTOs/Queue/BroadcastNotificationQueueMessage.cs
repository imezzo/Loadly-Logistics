using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Shared.DTOs.Queue;

public class BroadcastNotificationQueueMessage : QueueMessageBase
{
    public NotificationChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public List<long>? ModelIds { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
    public string? ClickAction { get; set; }
    public int Skip { get; set; }
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Public landing URL for this broadcast's attachments, if any. SMS channel appends it
    /// to the body so recipients can view attachments they can't receive by text.
    /// Null when the broadcast has no attachments or the share template isn't configured.
    /// </summary>
    public string? AttachmentLandingUrl { get; set; }

    public BroadcastNotificationQueueMessage()
    {
        Type = QueueMessageTypes.BroadcastNotification;
    }
}
