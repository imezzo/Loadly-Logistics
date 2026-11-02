namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class BroadcastNotificationAttachmentDto
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
