namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime RecordedAt { get; set; }
}
