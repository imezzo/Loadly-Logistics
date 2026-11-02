using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Collector.DTOs.Trips;


public class CollectTripDocumentsDto
{
    public long TripId { get; set; }
    public long CollectorId { get; set; }
    public bool Submit { get; set; }
    public List<TripDocumentDataDto> CollectorDocuments { get; set; } = new();
}

public class TripDocumentDataDto
{
    public long DocumentId { get; set; }
    public List<FileUpload>? Files { get; set; }
    public CollectorDocumentStatus Status { get; set; }
    public string? Note { get; set; }
}
