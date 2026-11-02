namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyDownloadTripDocumentDto
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;
    public long CompanyId { get; set; }
}
