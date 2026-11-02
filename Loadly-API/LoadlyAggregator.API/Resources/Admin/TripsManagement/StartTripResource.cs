using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Admin.TripsManagement;

public class StartTripResource
{
    public string Message { get; set; } = string.Empty;
    public TripWaypointResource NextWaypoint { get; set; } = null!;

    public static StartTripResource FromDto(TripWaypointDto nextWaypoint) => new()
    {
        Message = "trips.started_successfully",
        NextWaypoint = TripWaypointResource.FromDto(nextWaypoint)
    };
}
