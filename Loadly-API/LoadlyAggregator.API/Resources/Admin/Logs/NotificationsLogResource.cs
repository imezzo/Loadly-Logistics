using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Logs;

public class NotificationsLogResource
{
    public long Id { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Response { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public string? EventType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public NotificationLogRelation? RelationShip { get; set; }

    public static NotificationsLogResource FromDto(NotificationsLogDto dto) =>
        new NotificationsLogResource
        {
            Id = dto.Id,
            Channel = dto.Channel,
            Recipient = dto.Recipient,
            Response = dto.Response,
            Body = dto.Body,
            Status = dto.Status,
            EventType = dto.EventType?.ToString(),
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            RelationShip = new NotificationLogRelation
            {
                ModelType = dto.ModelType,
                ModelId = dto.ModelId
            }
        };

    public class NotificationLogRelation

    {
        public string? ModelType { get; set; }
        public long? ModelId { get; set; }
    }
}
