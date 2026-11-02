using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverCompleteTripEventData : RecordingData
{
    public int? Odometer { get; set; }
    public decimal? TollgatesTotalAmount { get; set; }
    public decimal? TollgatesWeightTotalAmount { get; set; }
    public string? OdometerImagePath { get; set; }
    public string? Note { get; set; }
    public List<string>? TollgateImagePaths { get; set; }
    public List<string>? TollgateWeightImagePaths { get; set; }
    public DriverLocationData? Location { get; set; }
    public WaypointSnapshot? Waypoint { get; set; }
}
