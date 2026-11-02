namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class BroadcastNotificationResultDto
{
    public bool Queued { get; set; }
    public string Message { get; set; } = string.Empty;
}
