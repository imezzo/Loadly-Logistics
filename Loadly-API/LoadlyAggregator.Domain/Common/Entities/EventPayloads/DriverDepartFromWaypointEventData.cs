using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverDepartFromWaypointEventData : RecordingData
{
    public string? Note { get; set; }
    public string? DeliveryNoteImagePath { get; set; }
    public int? Odometer { get; set; }
    public string? OdometerImagePath { get; set; }
    public DriverLocationData? Location { get; set; }
    public WaypointSnapshot? Waypoint { get; set; }
}
