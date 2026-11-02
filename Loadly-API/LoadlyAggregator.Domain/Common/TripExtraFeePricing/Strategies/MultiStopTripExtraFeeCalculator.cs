using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;

internal sealed class MultiStopTripExtraFeeCalculator : IExtraFeeCalculator
{
    public decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var includedStops = extraFee.Threshold ?? ExtraFeeMetadataRegistry.MultiStopDefaultIncludedStops;
        var extraStops = Math.Max(0, inputs.WaypointStopsCount - includedStops);
        if (extraStops <= 0)
            return 0m;

        var feePerStop = ExtraFeeCalculationOptionParser.GetValueOrZero(extraFee);
        return extraStops * feePerStop;
    }
}
