using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Resources.Driver.Trips;

public class SuggestOdometerReadingResource
{
    public string Message { get; set; } = string.Empty;
    public long ScanLogId { get; set; }
    public int OdometerReading { get; set; }
    public string? Unit { get; set; }
    public bool? IsDigital { get; set; }
    public string? ReadingQuality { get; set; }

    public static SuggestOdometerReadingResource FromDto(SuggestOdometerReadingResultDto dto) => new()
    {
        Message = TripMessageKeys.OdometerSuggestedSuccessfully,
        ScanLogId = dto.ScanLogId,
        OdometerReading = dto.SuggestedReading,
        Unit = dto.Unit,
        IsDigital = dto.IsDigital,
        ReadingQuality = dto.ReadingQuality
    };
}
