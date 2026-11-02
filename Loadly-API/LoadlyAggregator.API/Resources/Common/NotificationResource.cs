using LoadlyAggregator.Domain.Common.DTOs.Notifications;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Common;

public class NotificationResource
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int? EventType { get; set; }
    public NotificationResourceType? ResourceType { get; set; }
    public long? ResourceId { get; set; }
    public string? ClickAction { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? BroadcastNotificationId { get; set; }
    public List<NotificationAttachmentResource> Attachments { get; set; } = new();

    public static NotificationResource FromDto(NotificationDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Body = dto.Body,
        EventType = (int?)dto.EventType,
        ResourceType = dto.ResourceType,
        ResourceId = dto.ResourceId,
        ClickAction = dto.ClickAction,
        IsRead = dto.IsRead,
        ReadAt = dto.ReadAt,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        BroadcastNotificationId = dto.BroadcastNotificationId,
        Attachments = dto.Attachments.Select(NotificationAttachmentResource.FromDto).ToList()
    };
}

public class NotificationAttachmentResource
{
    public long Id { get; set; }
    public string Url { get; set; } = string.Empty;

    public static NotificationAttachmentResource FromDto(NotificationAttachmentDto dto) => new()
    {
        Id = dto.Id,
        Url = dto.Url
    };
}
