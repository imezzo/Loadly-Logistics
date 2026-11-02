using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum HelpersFeeCalculation
{
    [Display("Per Hour", "بالساعة")]
    PerHour = 1,
    [Display("Per Trip", "بالرحلة")]
    PerTrip = 2
}
