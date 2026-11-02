using LoadlyAggregator.Domain.Common.DTOs;

namespace LoadlyAggregator.API.Resources.Common;

public class PublicBroadcastNotificationResource
{
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public List<PublicBroadcastNotificationAttachmentResource> Attachments { get; set; } = new();

    public static PublicBroadcastNotificationResource FromDto(PublicBroadcastNotificationDto dto) => new()
    {
        Subject = dto.Subject,
        Body = dto.Body,
        SentAt = dto.SentAt,
        Attachments = dto.Attachments
            .Select(PublicBroadcastNotificationAttachmentResource.FromDto)
            .ToList()
    };
}

public class PublicBroadcastNotificationAttachmentResource
{
    public long Id { get; set; }
    public string Url { get; set; } = string.Empty;

    public static PublicBroadcastNotificationAttachmentResource FromDto(PublicBroadcastNotificationAttachmentDto dto) => new()
    {
        Id = dto.Id,
        Url = dto.Url
    };
}
