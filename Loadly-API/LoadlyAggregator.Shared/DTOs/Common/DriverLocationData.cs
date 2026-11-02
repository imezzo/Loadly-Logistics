namespace LoadlyAggregator.Shared.DTOs.Common;

/// <summary>
/// Represents driver's current location extracted from request headers
/// </summary>
public class DriverLocationData
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Accuracy { get; set; }
    public decimal? Heading { get; set; }
    public decimal? Speed { get; set; }
    public DateTime RecordedAt { get; set; }
}
