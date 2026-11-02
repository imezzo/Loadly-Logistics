namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class DriverDownloadTripDocumentDto
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;
    public long DriverId { get; set; }
}
