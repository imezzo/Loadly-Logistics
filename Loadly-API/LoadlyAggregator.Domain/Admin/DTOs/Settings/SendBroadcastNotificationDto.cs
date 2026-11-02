using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class SendBroadcastNotificationDto
{
    public BroadcastChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public List<long> ModelIds { get; set; } = [];
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? ClickAction { get; set; }
    public List<FileUpload>? Attachments { get; set; }
}
