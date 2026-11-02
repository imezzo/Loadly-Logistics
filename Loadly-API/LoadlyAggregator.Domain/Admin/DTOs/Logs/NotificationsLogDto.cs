using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Logs;

public class NotificationsLogDto
{
    public long Id { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Response { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public NotificationEventType? EventType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? ModelId { get; set; }
    public string? ModelType { get; set; }

    public static NotificationsLogDto FromEntity(NotificationsLog entity)
    {
        return new NotificationsLogDto
        {
            Id = entity.Id,
            Channel = entity.Channel,
            Recipient = entity.Recipient,
            Response = entity.Response,
            Body = entity.Body,
            Status = entity.Status,
            EventType = entity.EventType,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ModelId = entity.ModelId,
            ModelType = entity.ModelType
        };
    }
}

public class GetNotificationsLogsDto : BasePagedRequest
{
    public GetNotificationsLogsDto()
    {
        PageSize = 15;
    }

    public int? Channel { get; set; }
    public int? Status { get; set; }
    public string? Recipient { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
