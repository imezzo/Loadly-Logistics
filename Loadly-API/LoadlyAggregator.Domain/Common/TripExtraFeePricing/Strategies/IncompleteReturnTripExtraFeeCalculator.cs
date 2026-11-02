using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class IncompleteReturnTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var option = ExtraFeeCalculationOptionParser.ToIncompleteReturnOption(extraFee.CalculationOption);
        return option switch
        {
            IncompleteReturnCalculation.NormalTripCostOnly => 0m,
            IncompleteReturnCalculation.HalfRound => inputs.BaseCharge * 0.5m,
            IncompleteReturnCalculation.FullRound => inputs.BaseCharge,
            IncompleteReturnCalculation.CustomAmount => ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee),
            _ => 0m
        };
    }
}
