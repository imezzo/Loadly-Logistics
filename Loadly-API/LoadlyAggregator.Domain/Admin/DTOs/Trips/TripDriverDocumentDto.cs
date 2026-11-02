using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripDriverDocumentDto
{
    public TripDriverDocumentType Type { get; set; }
    public List<string> Paths { get; set; } = [];
    public List<string> FileUrls { get; set; } = [];
    public long RecordingId { get; set; }
    public long? WaypointId { get; set; }
    public DateTime CreatedAt { get; set; }
}
