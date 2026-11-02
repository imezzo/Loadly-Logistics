using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Logs;

public class OdometerScanLogDto
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

    public static OdometerScanLogDto FromEntity(
        OdometerScanLog entity,
        string? imageUrl,
        bool includeRawResponse = false) =>
        new()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            ModelId = entity.ModelId,
            ModelType = entity.ModelType,
            TripId = entity.TripId,
            ScanContext = entity.ScanContext,
            ImagePath = entity.ImagePath,
            ImageUrl = imageUrl,
            SuggestedReading = entity.SuggestedReading,
            Unit = entity.Unit,
            IsDigital = entity.IsDigital,
            ReadingQuality = entity.ReadingQuality,
            IsScreen = entity.IsScreen,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage,
            Provider = entity.Provider,
            Model = entity.Model,
            TotalTokens = entity.TotalTokens,
            LatencyMs = entity.LatencyMs,
            RawResponse = includeRawResponse ? entity.RawResponse : null,
            CorrelationId = entity.CorrelationId,
            PreviousScanLogId = entity.PreviousScanLogId
        };
}

public class GetOdometerScanLogsDto : BasePagedRequest
{
    public GetOdometerScanLogsDto()
    {
        PageSize = 15;
    }

    public long? TripId { get; set; }
    public long? ModelId { get; set; }
    public string? ModelType { get; set; }
    public int? Status { get; set; }
    public int? ScanContext { get; set; }
    public bool? IsScreen { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetOdometerScanLogDto
{
    public long Id { get; set; }
}
