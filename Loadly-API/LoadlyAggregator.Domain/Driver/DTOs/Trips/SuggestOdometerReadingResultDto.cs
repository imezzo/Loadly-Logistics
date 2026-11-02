namespace LoadlyAggregator.Domain.Driver.DTOs.Trips;

public class SuggestOdometerReadingResultDto
{
    public long ScanLogId { get; set; }
    public int SuggestedReading { get; set; }
    public string? Unit { get; set; }
    public bool? IsDigital { get; set; }
    public string? ReadingQuality { get; set; }
}
