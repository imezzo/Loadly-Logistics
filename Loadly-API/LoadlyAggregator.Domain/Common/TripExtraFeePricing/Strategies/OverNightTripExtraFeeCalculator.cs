using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class OverNightTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs) =>
        ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee);
}
