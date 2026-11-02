namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class TripRouteSyncPayloadDto
{
    public int PointCount { get; set; }
    public long EpochSeconds { get; set; }
    public List<int> DeltaTimeSeconds { get; set; } = [];
    public List<int> LatitudeScaled { get; set; } = [];
    public List<int> LongitudeScaled { get; set; } = [];
    public List<int> Speed { get; set; } = [];
    public List<int> HeadingDegrees { get; set; } = [];
    public List<int> AccuracyCentimeters { get; set; } = [];
    public List<int> DistanceSinceLastMeters { get; set; } = [];
}
