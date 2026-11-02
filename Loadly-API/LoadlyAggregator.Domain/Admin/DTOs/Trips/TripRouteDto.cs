using System.Text.Json;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripRouteDto
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public decimal DistanceMeters { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int GpsPacketCount { get; set; }
    public int PointCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public JsonElement Route { get; set; }
}
