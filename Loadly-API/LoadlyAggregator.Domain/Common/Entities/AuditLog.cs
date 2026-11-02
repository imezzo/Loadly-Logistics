using System.Text.Json;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Entities;

public class AuditLog : IEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long? EntityId { get; set; }
    public long? UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public JsonDocument? OldValues { get; set; }
    public JsonDocument? NewValues { get; set; }
    public List<string>? ChangeSummary { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
}
