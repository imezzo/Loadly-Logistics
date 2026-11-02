using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Logs;

public class OdometerScanLogResource
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? ModelId { get; set; }
    public string? ModelType { get; set; }
    public long? TripId { get; set; }
    public OdometerScanContext ScanContext { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
    public int? SuggestedReading { get; set; }
    public string? Unit { get; set; }
    public bool? IsDigital { get; set; }
    public string? ReadingQuality { get; set; }
    public bool IsScreen { get; set; }
    public OdometerScanStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? Model { get; set; }
    public int? TotalTokens { get; set; }
    public int? LatencyMs { get; set; }
    public string? RawResponse { get; set; }
    public string? CorrelationId { get; set; }
    public long? PreviousScanLogId { get; set; }

    public static OdometerScanLogResource FromDto(OdometerScanLogDto dto) => new()
    {
        Id = dto.Id,
        CreatedAt = dto.CreatedAt,
        ModelId = dto.ModelId,
        ModelType = dto.ModelType,
        TripId = dto.TripId,
        ScanContext = dto.ScanContext,
        ImagePath = dto.ImagePath,
        ImageUrl = dto.ImageUrl,
        SuggestedReading = dto.SuggestedReading,
        Unit = dto.Unit,
        IsDigital = dto.IsDigital,
        ReadingQuality = dto.ReadingQuality,
        IsScreen = dto.IsScreen,
        Status = dto.Status,
        ErrorMessage = dto.ErrorMessage,
        Provider = dto.Provider,
        Model = dto.Model,
        TotalTokens = dto.TotalTokens,
        LatencyMs = dto.LatencyMs,
        RawResponse = dto.RawResponse,
        CorrelationId = dto.CorrelationId,
        PreviousScanLogId = dto.PreviousScanLogId
    };
}
