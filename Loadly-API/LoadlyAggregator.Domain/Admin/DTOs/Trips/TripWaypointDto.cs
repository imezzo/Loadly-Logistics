using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

/// <summary>
/// Waypoint payload for trip create/update and response.
/// </summary>
public class TripWaypointDto
{
    public long Id { get; set; }
    public int Sequence { get; set; }
    public TripWaypointStopType StopType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AddressName { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? TaskDetails { get; set; }
    public TripWaypointStatus Status { get; set; }
}
