using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.DTOs.Notifications;

public class DriverListNotificationsDto : BasePagedRequest
{
    public long DriverId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}
