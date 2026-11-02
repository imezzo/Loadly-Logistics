using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class BroadcastNotificationDto
{
    public long Id { get; set; }
    public long AddedBy { get; set; }
    public BroadcastChannel Channel { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public List<long> ModelIds { get; set; } = new();
    public List<BroadcastNotificationModelDto> Models { get; set; } = new();
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BroadcastNotificationAttachmentDto> Attachments { get; set; } = new();

    public static BroadcastNotificationDto FromEntity(BroadcastNotification entity)
    {
        List<BroadcastNotificationModelDto> models = new();
        if (!string.IsNullOrWhiteSpace(entity.ModelIds))
        {
            try
            {
                List<long>? ids = JsonSerializer.Deserialize<List<long>>(entity.ModelIds);
                if (ids is not null)
                {
                    models = ids.ConvertAll(id => new BroadcastNotificationModelDto { Id = id });
                }
            }
            catch (JsonException)
            {
                // shortcut: swallow malformed persisted JSON, upgrade when audit data confirms it never happens
            }
        }

        return new BroadcastNotificationDto
        {
            Id = entity.Id,
            AddedBy = entity.AddedBy,
            Channel = entity.Channel,
            ModelType = entity.ModelType,
            ModelIds = models.ConvertAll(m => m.Id),
            Models = models,
            Subject = entity.Subject,
            Body = entity.Body,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Attachments = entity.Attachments
                .Select(a => new BroadcastNotificationAttachmentDto
                {
                    Id = a.Id,
                    Path = a.Path
                })
                .ToList()
        };
    }
}
