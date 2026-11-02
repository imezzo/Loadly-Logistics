using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class WaypointSnapshotExtensions
{
    public static WaypointSnapshot ToSnapshot(this TripWaypoint waypoint) => new()
    {
        Id = waypoint.Id,
        Sequence = waypoint.Sequence,
        AddressName = waypoint.AddressName,
        Latitude = waypoint.Latitude,
        Longitude = waypoint.Longitude,
        Status = (int)waypoint.Status,
        StopType = (int)waypoint.StopType
    };
}
