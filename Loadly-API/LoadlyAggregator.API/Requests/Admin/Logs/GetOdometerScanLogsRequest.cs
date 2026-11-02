using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Logs;

public class GetOdometerScanLogsRequest : BasePagedRequest
{
    public const string Permission = "view_odometer_scan_log";

    public GetOdometerScanLogsRequest()
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

    public GetOdometerScanLogsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        TripId = TripId,
        ModelId = ModelId,
        ModelType = ModelType,
        Status = Status,
        ScanContext = ScanContext,
        IsScreen = IsScreen,
        CorrelationId = CorrelationId,
        StartDate = StartDate,
        EndDate = EndDate
    };
}
