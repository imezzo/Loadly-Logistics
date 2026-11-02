using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class LoadingUnloadingTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var option = ExtraFeeCalculationOptionParser.ToLoadingUnloadingOption(extraFee.CalculationOption);
        var value = ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee);
        return option switch
        {
            LoadingUnloadingCalculation.Included => 0m,
            LoadingUnloadingCalculation.Fixed => value,
            LoadingUnloadingCalculation.PerItem => inputs.LoadingUnloadingCompletions * value,
            LoadingUnloadingCalculation.TimeBased => inputs.BillableTripHours * value,
            _ => 0m
        };
    }
}
