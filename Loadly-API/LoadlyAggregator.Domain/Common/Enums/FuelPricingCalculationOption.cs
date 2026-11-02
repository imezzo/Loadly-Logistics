using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum FuelPricingCalculationOption
{
    [Display("Included in contract price", "مشمول في سعر العقد")]
    IncludedInContractPrice = 1,
    [Display("Calculated per trip", "محسوب لكل رحلة")]
    CalculatedPerTrip = 2
}
