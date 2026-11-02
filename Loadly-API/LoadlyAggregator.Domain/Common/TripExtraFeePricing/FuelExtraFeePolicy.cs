using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <summary>
/// Fuel inclusion/per-trip calculation rules and validation when building trip extra-fee inputs or computing fuel addon cost.
/// </summary>
internal static class FuelExtraFeePolicy
{
    internal static (decimal Amount, decimal CompanyShare) ResolveChargeAmounts(decimal fuelPrice, int? calculationOption)
    {
        if (calculationOption is not int option)
            throw new LogicalException("Fuel extra fee must have CalculationOption set (IncludedInContractPrice or CalculatedPerTrip).");

        return option switch
        {
            (int)FuelPricingCalculationOption.IncludedInContractPrice => (fuelPrice, fuelPrice),
            (int)FuelPricingCalculationOption.CalculatedPerTrip => (fuelPrice, 0m),
            _ => throw new LogicalException(
                $"Fuel extra fee has invalid CalculationOption ({option}); expected IncludedInContractPrice (1) or CalculatedPerTrip (2).")
        };
    }

    internal static decimal CalculateAddonCost(TripExtraFeeInputs inputs)
    {
        if (inputs.FuelUnitPricePerTrip is not decimal fuelUnitPrice)
            return 0m;

        var distanceKm = inputs.DistanceKm;
        if (distanceKm <= 0m)
            return 0m;

        var consumptionRate = inputs.FuelConsumptionRate ?? 0m;
        if (consumptionRate <= 0m)
            return 0m;

        if (fuelUnitPrice <= 0m)
            return 0m;

        return (distanceKm * fuelUnitPrice) / consumptionRate;
    }
}
