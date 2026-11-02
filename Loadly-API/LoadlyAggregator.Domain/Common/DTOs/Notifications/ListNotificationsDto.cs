using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Notifications;

public class ListNotificationsDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public NotificationEventType? EventType { get; set; }
    public NotificationEventType? Exclude { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
