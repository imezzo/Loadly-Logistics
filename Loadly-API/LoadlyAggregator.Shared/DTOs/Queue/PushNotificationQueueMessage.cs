using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Shared.DTOs.Queue;

public class PushNotificationQueueMessage : QueueMessageBase
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
    public string? ImageUrl { get; set; }
    public string? ClickAction { get; set; }

    public PushNotificationQueueMessage()
    {
        Type = QueueMessageTypes.PushNotification;
    }
}
