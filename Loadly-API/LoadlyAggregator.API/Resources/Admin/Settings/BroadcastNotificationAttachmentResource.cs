using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class BroadcastNotificationAttachmentResource
{
    public long Id { get; set; }
    public string Url { get; set; } = string.Empty;

    public static BroadcastNotificationAttachmentResource FromDto(BroadcastNotificationAttachmentDto dto) => new()
    {
        Id = dto.Id,
        Url = dto.Url
    };
}
