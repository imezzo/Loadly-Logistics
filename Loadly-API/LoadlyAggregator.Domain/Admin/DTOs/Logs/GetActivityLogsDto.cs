using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Logs;

public class GetActivityLogsDto : BasePagedRequest
{
    public GetActivityLogsDto()
    {
        PageSize = 15;
    }

    public string? EventType { get; set; }
    public string? EntityType { get; set; }
    public long? UserId { get; set; }
    public string? UserType { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
