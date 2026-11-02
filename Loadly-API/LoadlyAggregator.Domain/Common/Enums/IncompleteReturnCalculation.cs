using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum IncompleteReturnCalculation
{
    [Display("Standard One-Way", "تكلفة الذهاب فقط")]
    NormalTripCostOnly = 1,
    [Display("Half Round Trip", "نصف تكلفة الذهاب والعودة")]
    HalfRound = 2,
    [Display("Full Round Trip", "تكلفة ذهاب وعودة كاملة")]
    FullRound = 3,
    [Display("Fixed Amount", "مبلغ ثابت")]
    CustomAmount = 4
}
