using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class FuelTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee _, TripExtraFeeInputs inputs)
    {
        return FuelExtraFeePolicy.CalculateAddonCost(inputs);
    }
}
