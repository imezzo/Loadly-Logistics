namespace LoadlyAggregator.Domain.Common.Entities.EventPayloads;

public class DriverCancelTripEventData : RecordingData
{
    public string? CancellationReason { get; set; }
}
