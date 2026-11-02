using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripCustomsStatus
{
    [Display("None", "بدون جمارك")]
    None = 0,

    [Display("Pending Entrance", "في انتظار دخول الجمارك")]
    PendingEntrance = 1,

    [Display("Under Inspection", "قيد الفحص")]
    UnderInspection = 2,

    [Display("Cleared", "تم التخليص")]
    Cleared = 3
}
