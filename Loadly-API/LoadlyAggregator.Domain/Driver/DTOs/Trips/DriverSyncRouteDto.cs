namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverSyncRouteDto
{
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public decimal DistanceMeters { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int GpsPacketCount { get; set; }
    public TripRouteSyncPayloadDto Route { get; set; } = null!;
}
