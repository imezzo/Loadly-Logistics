using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class BroadcastNotificationItemResource
{
    public long Id { get; set; }
    public long AddedBy { get; set; }
    public BroadcastChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public List<long> ModelIds { get; set; } = new();
    public List<BroadcastNotificationModelResource> Models { get; set; } = new();
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BroadcastNotificationAttachmentResource> Attachments { get; set; } = new();

    public static BroadcastNotificationItemResource FromDto(BroadcastNotificationDto dto) => new()
    {
        Id = dto.Id,
        AddedBy = dto.AddedBy,
        Channel = dto.Channel,
        ModelType = dto.ModelType,
        ModelIds = dto.ModelIds,
        Models = dto.Models.Select(BroadcastNotificationModelResource.FromDto).ToList(),
        Subject = dto.Subject,
        Body = dto.Body,
        Status = dto.Status,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        Attachments = dto.Attachments
            .Select(BroadcastNotificationAttachmentResource.FromDto)
            .ToList()
    };
}

public class BroadcastNotificationModelResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static BroadcastNotificationModelResource FromDto(BroadcastNotificationModelDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name
    };
}
