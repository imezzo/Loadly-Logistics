using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripRouteResource
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

    public static TripRouteResource FromDto(TripRouteDto dto) =>
        new()
        {
            Id = dto.Id,
            TripId = dto.TripId,
            DriverId = dto.DriverId,
            DistanceMeters = dto.DistanceMeters,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            GpsPacketCount = dto.GpsPacketCount,
            PointCount = dto.PointCount,
            CreatedAt = dto.CreatedAt,
            Route = dto.Route
        };
}
