using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum WeightFeeCalculation
{
    [Display("Full Tons Only", "أطنان كاملة فقط")]
    FullTonOnly = 1,
    [Display("Partial Tons Included", "تقريب لأعلى للكسر")]
    RoundUpPartial = 2,
    [Display("Prorated", "تسعير نسبي بالوزن")]
    Prorated = 3
}
