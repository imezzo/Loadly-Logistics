using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;

public class ArriveAtWaypointDto
{
    public long TripId { get; set; }
    public long WaypointId { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
