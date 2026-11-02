using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverEntersCustomsEventData : RecordingData
{
    public string? Note { get; set; }
    public List<string>? CustomsDocumentsPaths { get; set; }
    public DriverLocationData? Location { get; set; }
}
