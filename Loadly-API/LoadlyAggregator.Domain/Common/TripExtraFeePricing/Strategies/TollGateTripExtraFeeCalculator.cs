using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class TollGateTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var option = ExtraFeeCalculationOptionParser.ToTollGateOption(extraFee.CalculationOption);

        if (extraFee.AllowOverride == true && inputs.ManualTollAmount.HasValue)
            return inputs.ManualTollAmount.Value;

        return option switch
        {
            TollGateCalculation.Included => inputs.TollgatesTotalAmount ?? 0m,
            TollGateCalculation.ClientPay => 0m,
            TollGateCalculation.Fixed => ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee),
            TollGateCalculation.VariablePerRoute => inputs.TollgatesTotalAmount
                ?? extraFee.Value
                ?? 0m,
            _ => 0m
        };
    }
}
