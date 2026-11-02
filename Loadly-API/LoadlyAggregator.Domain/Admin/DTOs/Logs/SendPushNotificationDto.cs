namespace LoadlyAggregator.Domain.Admin.DTOs.Logs;

public class SendPushNotificationDto
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
    public string? ImageUrl { get; set; }
    public string? ClickAction { get; set; }
    public string Domain { get; set; } = "default";
}
