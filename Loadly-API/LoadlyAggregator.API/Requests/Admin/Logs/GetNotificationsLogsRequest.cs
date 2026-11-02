using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Logs;

public class GetNotificationsLogsRequest : BasePagedRequest
{
    public const string Permission = "view_notifications_log";

    public GetNotificationsLogsRequest()
    {
        PageSize = 15;
    }

    public int? Channel { get; set; }
    public int? Status { get; set; }
    public string? Recipient { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetNotificationsLogsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Channel = Channel,
        Status = Status,
        Recipient = Recipient,
        StartDate = StartDate,
        EndDate = EndDate
    };
}
