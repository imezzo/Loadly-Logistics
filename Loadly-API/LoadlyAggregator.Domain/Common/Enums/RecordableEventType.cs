using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum RecordableEventType
{
    [Display("Loading Started", "بدأ التحميل")]
    WAYPOINT_LOADING_STARTED = 2,
    [Display("Loading Completed", "إكتمل التحميل")]
    WAYPOINT_LOADING_COMPLETED = 3,
    [Display("Trip Started", "بدأت الرحلة")]
    TRIP_STARTED = 4,
    [Display("Waypoint Arrived", "الوصول إلى نقطة المسار")]
    WAYPOINT_ARRIVED = 5,
    [Display("Waypoint Departed", "مغادرة نقطة المسار")]
    WAYPOINT_DEPARTED = 6,
    [Display("Trip Completed", "إكتملت الرحلة")]
    TRIP_COMPLETED = 7,
    [Display("Unloading Started", "بدأ التفريغ")]
    WAYPOINT_UNLOADING_STARTED = 8,
    [Display("Unloading Completed", "إكتمل التفريغ")]
    WAYPOINT_UNLOADING_COMPLETED = 9,
    [Display("Trip Cancelled", "ألغيت الرحلة")]
    TRIP_CANCELLED = 10,
    [Display("Entered Customs", "دخول إلى الجمارك")]
    TRIP_CUSTOMS_ENTERED = 11,
    [Display("Exited Customs", "خروج من الجمارك")]
    TRIP_CUSTOMS_EXITED = 12
}
