using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

/// <summary>
/// Each row is a specific task at a specific Google Maps location.
/// </summary>
public class TripWaypoint : BaseEntity
{
    public long TripId { get; set; }
    public int Sequence { get; set; }
    public TripWaypointStopType StopType { get; set; }
    public string? GooglePlaceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string AddressName { get; set; } = string.Empty;
    public string? TaskDetails { get; set; }
    public TripWaypointStatus Status { get; set; } = TripWaypointStatus.Pending;

    public virtual Trip Trip { get; set; } = null!;
}
