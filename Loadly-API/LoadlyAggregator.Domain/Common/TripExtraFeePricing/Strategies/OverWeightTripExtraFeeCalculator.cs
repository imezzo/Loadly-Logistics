using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class OverWeightTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var ratePerTon = inputs.OverWeightPricePerTon ?? 0m;
        var maxPayloadTons = inputs.MaxPayloadTons ?? 0m;
        if (ratePerTon <= 0m || maxPayloadTons <= 0m || inputs.CargoWeightTons <= maxPayloadTons)
            return 0m;

        var excessTons = inputs.CargoWeightTons - maxPayloadTons;
        var option = ExtraFeeCalculationOptionParser.ToWeightFeeOption(extraFee.CalculationOption);
        var chargeableTons = option switch
        {
            WeightFeeCalculation.FullTonOnly => Math.Floor(excessTons),
            WeightFeeCalculation.RoundUpPartial => Math.Ceiling(excessTons),
            _ => excessTons
        };
        return chargeableTons * ratePerTon;
    }
}
