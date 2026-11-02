using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractConsumptionTypes
{
    [Display("Trips", "رحلات")]
    Trips = 1,
    [Display("Kilometers", "كيلومترات")]
    KMs = 2,
    [Display("Cargo Weights", "وزن الشحنة")]
    CARGO_WEIGHTS = 3,
    [Display("Destination Fixed Price", "سعر ثابت للوجهة")]
    DESTINATION_FIXED_PRICE = 4,
    [Display("Cargo Price Commission", "عمولة من قيمة البضاعة")]
    CARGO_PRICE_COMMISSION = 5,
    [Display("Spot Pricing", "تسعير فوري")]
    SPOT_PRICING = 6
}
