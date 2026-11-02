using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverCompleteLoadingCargoEventData : RecordingData
{
    public string? Note { get; set; }
    public string? ReceiptNoteImagePath { get; set; }
    public List<string>? ShipmentImagePaths { get; set; }
    public DriverLocationData? Location { get; set; }
    public WaypointSnapshot? Waypoint { get; set; }
}
