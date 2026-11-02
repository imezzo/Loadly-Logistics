using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Shared.DTOs.Queue;

public class EmailQueueMessage : QueueMessageBase
{
    public string? From { get; set; }
    public string? FromName { get; set; }
    public string To { get; set; } = string.Empty;
    public string? ToName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = false;

    public EmailQueueMessage()
    {
        Type = QueueMessageTypes.Email;
    }
}
