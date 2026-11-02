using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum LoadingUnloadingCalculation
{
    [Display("Included in Base Rate", "ضمن السعر الأساسي")]
    Included = 1,
    [Display("Separate Charge per trip", "رسوم منفصلة لكل رحلة")]
    Fixed = 2,
    [Display("Per Item/Pallet Charge", "لكل قطعة أو طبلية")]
    PerItem = 3,
    [Display("Time-based Charge", "بالساعة")]
    TimeBased = 4
}
