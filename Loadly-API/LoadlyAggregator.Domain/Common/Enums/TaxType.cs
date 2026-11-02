using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum TaxType
{
    [Display("Fixed", "ثابت")]
    Fixed = 0,
    [Display("Percentage", "نسبة مئوية")]
    Percentage = 1
}
