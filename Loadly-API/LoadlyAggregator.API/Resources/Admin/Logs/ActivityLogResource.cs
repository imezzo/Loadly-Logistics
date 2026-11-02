using LoadlyAggregator.Domain.Admin.DTOs.Logs;

namespace LoadlyAggregator.API.Resources.Admin.Logs;

public class ActivityLogResource
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

    public static ActivityLogResource FromDto(ActivityLogDto dto) =>
        new ActivityLogResource
        {
            Id = dto.Id,
            EventType = dto.EventType,
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            UserId = dto.UserId,
            UserType = dto.UserType,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
            ChangeSummary = dto.ChangeSummary,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
            CorrelationId = dto.CorrelationId,
            CreatedAt = dto.CreatedAt
        };
}
