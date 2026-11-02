using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class HelpersTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var option = ExtraFeeCalculationOptionParser.ToHelpersFeeOption(extraFee.CalculationOption);
        var includedHelpers = (int)ExtraFeeCalculationOptionParser.GetThresholdOrZero(extraFee);
        var extraHelpers = Math.Max(0, inputs.HelpersCount - includedHelpers);
        if (extraHelpers <= 0)
            return 0m;

        var value = ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee);
        return option switch
        {
            HelpersFeeCalculation.PerHour => extraHelpers * inputs.BillableTripHours * value,
            HelpersFeeCalculation.PerTrip => extraHelpers * value,
            _ => 0m
        };
    }
}
