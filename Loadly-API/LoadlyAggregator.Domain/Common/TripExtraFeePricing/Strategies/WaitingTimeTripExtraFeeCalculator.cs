using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class WaitingTimeTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var option = ExtraFeeCalculationOptionParser.ToWaitingTimeOption(extraFee.CalculationOption);
        var freeMinutes = (int)ExtraFeeCalculationOptionParser.GetThresholdOrZero(extraFee);
        var chargeableMinutes = Math.Max(0, inputs.WaitingMinutesAtStops - freeMinutes);
        if (chargeableMinutes <= 0)
            return 0m;

        var hourlyRate = ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee);
        if (hourlyRate <= 0)
            return 0m;

        var chargeableHours = option switch
        {
            WaitingTimeFeeCalculation.ProratedMinutes => chargeableMinutes / 60m,
            WaitingTimeFeeCalculation.RoundToHour => Math.Ceiling(chargeableMinutes / 60m),
            WaitingTimeFeeCalculation.Round15MinBlock => Math.Ceiling(chargeableMinutes / 15m) * 15m / 60m,
            _ => chargeableMinutes / 60m
        };
        return chargeableHours * hourlyRate;
    }
}
