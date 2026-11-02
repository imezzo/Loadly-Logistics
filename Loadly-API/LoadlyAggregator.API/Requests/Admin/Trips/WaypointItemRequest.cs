using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

/// <summary>
/// Single waypoint for trip create/update requests.
/// </summary>
public class WaypointItemRequest
{
    public long? Id { get; set; }
    public int Sequence { get; set; }
    public TripWaypointStopType StopType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AddressName { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? TaskDetails { get; set; }
}
