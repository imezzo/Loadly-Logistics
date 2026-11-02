using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverStartLoadingCargoDto
{
    public long TripId { get; set; }
    public long WaypointId { get; set; }
    public long DriverId { get; set; }
    public DriverLocationData? CurrentLocation { get; set; }
}
