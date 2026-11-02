using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TripLifecycleActor
{
    [Display("Driver-Only", "السائق")]
    Driver = 1,

    [Display("Back-Office", "الإدارة")]
    Dispatcher = 2,

    [Display("Assisted", "السائق و الأدراة")]
    Assisted = 3
}
