using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverExitsCustomsEventData : RecordingData
{
    public decimal? CustomsTotalAmount { get; set; }
    public string? Note { get; set; }
    public List<string>? CustomsReceiptPaths { get; set; }
    public DriverLocationData? Location { get; set; }
}
