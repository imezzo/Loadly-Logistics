using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <summary>
/// Builds <see cref="TripExtraFeeInputs"/> from trip, contract vehicle type, and contract extra fees (telemetry + fuel policy).
/// </summary>
internal static class TripExtraFeeInputFactory
{
    internal static TripExtraFeeInputs CreateForTrip(
        Trip trip,
        decimal baseCharge,
        ContractTripPricingTerms pricing,
        string contractReferenceNumber,
        IReadOnlyList<ContractExtraFee> contractExtraFees)
    {
        var fuelUnitPrice = trip.Vehicle?.FuelType?.UnitPrice;
        decimal? vehicleFuelUnitPrice = fuelUnitPrice is > 0m ? fuelUnitPrice : null;

        var recordingMetrics = TripRecordingMetricsBuilder.Build(trip.Recordings);
        var distanceKm = trip.ContractDistanceKm ?? 0m;
        var stops = trip.Waypoints?.Count ?? 0;

        return new TripExtraFeeInputs(
            baseCharge,
            distanceKm,
            stops,
            trip.ActualCargoWeightTons ?? 0m,
            recordingMetrics.WaitingMinutesAtStops,
            trip.HelpersCount ?? 0,
            recordingMetrics.LoadingUnloadingCompletions,
            recordingMetrics.BillableTripHours,
            manualTollAmount: null,
            tollgatesTotalAmount: recordingMetrics.TollgatesTotalAmount,
            tollgatesWeightTotalAmount: recordingMetrics.TollgatesWeightTotalAmount,
            fuelConsumptionRate: pricing.ConsumptionRate,
            vehicleFuelUnitPrice: vehicleFuelUnitPrice,
            maxPayloadTons: pricing.MaxPayloadTons,
            overWeightPricePerTon: pricing.OverWeightPricePerTon);
    }
}
