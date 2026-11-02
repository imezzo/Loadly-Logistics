namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorDownloadTripDocumentDto
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;
    public long ContractorId { get; set; }
}
