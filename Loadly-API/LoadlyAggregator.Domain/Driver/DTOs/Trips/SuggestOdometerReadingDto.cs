using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class SuggestOdometerReadingDto
{
    public long TripId { get; set; }
    public long ModelId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public required FileUpload OdometerImage { get; set; }
    public OdometerScanContext ScanContext { get; set; } = OdometerScanContext.Suggest;
    public long? PreviousScanLogId { get; set; }
    public string? CorrelationId { get; set; }
}
