namespace LoadlyAggregator.Shared.DTOs.Common;

/// <summary>
/// Snapshot of waypoint data at the time of a recording event.
/// </summary>
public class WaypointSnapshot
{
    public long Id { get; set; }
    public int Sequence { get; set; }
    public string? AddressName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int Status { get; set; }
    public int StopType { get; set; }  // 0 = Pickup, 1 = Dropoff
}
