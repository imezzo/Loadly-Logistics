using System.Text.Json;
using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class TripRouteSync : BaseEntity
{
    public long TripId { get; set; }
    public long DriverId { get; set; }
    public decimal DistanceMeters { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int GpsPacketCount { get; set; }
    public int PointCount { get; set; }
    public JsonDocument Payload { get; set; } = null!;
}
