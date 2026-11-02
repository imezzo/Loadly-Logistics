using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Entities;

public class OdometerScanLog : IEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public long? ModelId { get; set; }
    public string? ModelType { get; set; }

    public long? TripId { get; set; }
    public OdometerScanContext ScanContext { get; set; } = OdometerScanContext.Suggest;

    public string? ImagePath { get; set; }
    public int? SuggestedReading { get; set; }
    public string? Unit { get; set; }
    public bool? IsDigital { get; set; }
    public string? ReadingQuality { get; set; }
    public bool IsScreen { get; set; }
    public OdometerScanStatus Status { get; set; } = OdometerScanStatus.Failure;
    public string? ErrorMessage { get; set; }

    public string Provider { get; set; } = string.Empty;
    public string? Model { get; set; }
    public int? TotalTokens { get; set; }
    public int? LatencyMs { get; set; }
    public string? RawResponse { get; set; }

    public string? CorrelationId { get; set; }
    public long? PreviousScanLogId { get; set; }
}
