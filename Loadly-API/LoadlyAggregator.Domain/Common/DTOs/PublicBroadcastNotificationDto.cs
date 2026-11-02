namespace LoadlyAggregator.Domain.Common.DTOs;

public class PublicBroadcastNotificationDto
{
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public List<PublicBroadcastNotificationAttachmentDto> Attachments { get; set; } = new();
}

public class PublicBroadcastNotificationAttachmentDto
{
    public long Id { get; set; }
    public string Url { get; set; } = string.Empty;
}
