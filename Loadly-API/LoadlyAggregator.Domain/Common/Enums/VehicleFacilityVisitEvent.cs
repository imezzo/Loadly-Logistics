using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum VehicleFacilityVisitEvent
{
    [Display("Enters Location", "دخول الموقع")]
    ENTERS_LOCATION = 1,
    [Display("Exits Location", "خروج من الموقع")]
    EXITS_LOCATION = 2
}
