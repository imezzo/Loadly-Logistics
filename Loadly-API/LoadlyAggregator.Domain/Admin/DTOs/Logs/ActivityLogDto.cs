using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Admin.DTOs.Logs;

public class ActivityLogDto
{
    public long Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long? EntityId { get; set; }
    public long? UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public List<string>? ChangeSummary { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ActivityLogDto FromEntity(AuditLog entity)
    {
        return new ActivityLogDto
        {
            Id = entity.Id,
            EventType = entity.EventType,
            EntityType = entity.EntityType,
            EntityId = entity.EntityId,
            UserId = entity.UserId,
            UserType = entity.UserType,
            OldValues = entity.OldValues?.RootElement.GetRawText(),
            NewValues = entity.NewValues?.RootElement.GetRawText(),
            ChangeSummary = entity.ChangeSummary,
            IpAddress = entity.IpAddress,
            UserAgent = entity.UserAgent,
            CorrelationId = entity.CorrelationId,
            CreatedAt = entity.CreatedAt
        };
    }
}
