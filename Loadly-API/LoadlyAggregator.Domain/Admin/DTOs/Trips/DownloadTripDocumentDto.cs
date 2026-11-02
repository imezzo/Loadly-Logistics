namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class DownloadTripDocumentDto
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;
}
