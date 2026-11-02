namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <summary>
/// Snapshot of trip/contract context passed to contract extra-fee amount strategies.
/// </summary>
public sealed class TripExtraFeeInputs(
    decimal baseCharge,
    decimal distanceKm,
    int stopsCount,
    decimal cargoWeightTons,
    int waitingMinutesAtStops,
    int helpersCount,
    int loadingUnloadingCompletions,
    decimal tripHours,
    decimal? manualTollAmount,
    decimal? tollgatesTotalAmount,
    decimal? tollgatesWeightTotalAmount,
    decimal? fuelConsumptionRate,
    decimal? vehicleFuelUnitPrice,
    decimal? maxPayloadTons,
    decimal? overWeightPricePerTon)
{
    public decimal BaseCharge { get; } = baseCharge;
    public decimal DistanceKm { get; } = distanceKm;
    public int WaypointStopsCount { get; } = stopsCount;
    public decimal CargoWeightTons { get; } = cargoWeightTons;
    public int WaitingMinutesAtStops { get; } = waitingMinutesAtStops;
    public int HelpersCount { get; } = helpersCount;
    public int LoadingUnloadingCompletions { get; } = loadingUnloadingCompletions;
    public decimal BillableTripHours { get; } = tripHours;
    public decimal? ManualTollAmount { get; } = manualTollAmount;
    public decimal? TollgatesTotalAmount { get; } = tollgatesTotalAmount;
    public decimal? TollgatesWeightTotalAmount { get; } = tollgatesWeightTotalAmount;
    public decimal? FuelConsumptionRate { get; } = fuelConsumptionRate;
    public decimal? FuelUnitPricePerTrip { get; } = vehicleFuelUnitPrice;
    public decimal? MaxPayloadTons { get; } = maxPayloadTons;
    public decimal? OverWeightPricePerTon { get; } = overWeightPricePerTon;
}
