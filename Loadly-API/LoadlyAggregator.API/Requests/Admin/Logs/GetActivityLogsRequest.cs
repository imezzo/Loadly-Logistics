using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Logs;

public class GetActivityLogsRequest : BasePagedRequest
{
    public const string Permission = "view_activity_log";

    public GetActivityLogsRequest()
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

    public GetActivityLogsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        EventType = EventType,
        EntityType = EntityType,
        UserId = UserId,
        UserType = UserType,
        CorrelationId = CorrelationId,
        StartDate = StartDate,
        EndDate = EndDate
    };
}
