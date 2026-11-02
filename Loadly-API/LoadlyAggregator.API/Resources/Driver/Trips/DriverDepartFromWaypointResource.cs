using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Driver.Trips;

public class DriverDepartFromWaypointResource
{
    public string Message { get; set; } = string.Empty;
    public TripWaypointResource? NextWaypoint { get; set; }

    public static DriverDepartFromWaypointResource FromDto(TripWaypointDto? nextWaypoint) => new()
    {
        Message = "trips.waypoint_departed_successfully",
        NextWaypoint = nextWaypoint != null ? TripWaypointResource.FromDto(nextWaypoint) : null
    };
}
