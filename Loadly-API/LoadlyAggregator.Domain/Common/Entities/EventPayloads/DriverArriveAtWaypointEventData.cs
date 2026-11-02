using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverArriveAtWaypointEventData : RecordingData
{
    public DriverLocationData? Location { get; set; }
    public WaypointSnapshot? Waypoint { get; set; }
}
