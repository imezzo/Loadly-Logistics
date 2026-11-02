using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Driver.Trips;

public class DriverStartTripResource
{
    public string Message { get; set; } = string.Empty;
    public TripWaypointResource NextWaypoint { get; set; } = null!;

    public static DriverStartTripResource FromDto(TripWaypointDto nextWaypoint) => new()
    {
        Message = "trips.started_successfully",
        NextWaypoint = TripWaypointResource.FromDto(nextWaypoint)
    };
}
