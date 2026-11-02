using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Notifications;

public class NotificationDto
{
    public long Id { get; set; }
    public long ModelId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationEventType? EventType { get; set; }
    public NotificationResourceType? ResourceType { get; set; }
    public long? ResourceId { get; set; }
    public string? ClickAction { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? BroadcastNotificationId { get; set; }
    public List<NotificationAttachmentDto> Attachments { get; set; } = new();

    public static NotificationDto FromEntity(Notification entity) => new()
    {
        Id = entity.Id,
        ModelId = entity.ModelId,
        ModelType = entity.ModelType,
        Title = entity.Title,
        Body = entity.Body,
        EventType = entity.EventType,
        ResourceType = entity.ResourceType,
        ResourceId = entity.ResourceId,
        ClickAction = entity.ClickAction,
        IsRead = entity.ReadAt is not null,
        ReadAt = entity.ReadAt,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        BroadcastNotificationId = entity.BroadcastNotificationId,
        Attachments = entity.BroadcastNotification?.Attachments
            .Select(a => new NotificationAttachmentDto
            {
                Id = a.Id,
                Path = a.Path
            })
            .ToList() ?? new List<NotificationAttachmentDto>()
    };
}
