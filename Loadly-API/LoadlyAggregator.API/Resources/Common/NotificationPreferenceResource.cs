using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.API.Resources.Common;

public class NotificationPreferenceResource
{
    public string EventType { get; set; } = string.Empty;
    public string EventTypeNameEn { get; set; } = string.Empty;
    public string EventTypeNameAr { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string ChannelNameEn { get; set; } = string.Empty;
    public string ChannelNameAr { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }

    public static NotificationPreferenceResource FromDto(NotificationPreferenceItemDto dto) =>
        new()
        {
            EventType = dto.EventType.ToString(),
            EventTypeNameEn = NotificationEventTypeDisplayNames.GetNameEn(dto.EventType),
            EventTypeNameAr = NotificationEventTypeDisplayNames.GetNameAr(dto.EventType),
            Channel = dto.Channel.ToString(),
            ChannelNameEn = NotificationChannelDisplayNames.GetNameEn(dto.Channel),
            ChannelNameAr = NotificationChannelDisplayNames.GetNameAr(dto.Channel),
            IsEnabled = dto.IsEnabled
        };
}
