using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripWaypointStopType
{
    [Display("Pickup", "نقطة التحميل")]
    Pickup = 0,
    [Display("Dropoff", "نقطة التعتيق")]
    Dropoff = 1,
    [Display("Depot", "نقطة العودة")]
    Depot = 2
}
