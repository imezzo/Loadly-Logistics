using System.Text.Json;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripRecordingDto
{
    public long Id { get; set; }
    public int EventType { get; set; }
    public string EventName { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
    public string? AddedByModelType { get; set; }
    public JsonElement? Data { get; set; }
    public DateTime CreatedAt { get; set; }
}
