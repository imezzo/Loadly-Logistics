using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverStartUnloadingCargoEventData : RecordingData
{
    public DriverLocationData? Location { get; set; }
    public WaypointSnapshot? Waypoint { get; set; }
}
