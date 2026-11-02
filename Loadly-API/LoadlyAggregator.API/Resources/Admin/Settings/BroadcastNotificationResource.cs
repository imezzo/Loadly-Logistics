using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class BroadcastNotificationResource
{
    public bool Queued { get; set; }
    public string Message { get; set; } = string.Empty;

    public static BroadcastNotificationResource FromDto(BroadcastNotificationResultDto dto) => new()
    {
        Queued = dto.Queued,
        Message = dto.Message
    };
}
