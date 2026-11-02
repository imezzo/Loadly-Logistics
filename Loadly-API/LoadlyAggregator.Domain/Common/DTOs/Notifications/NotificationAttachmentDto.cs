namespace LoadlyAggregator.Domain.Common.DTOs.Notifications;

public class NotificationAttachmentDto
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
