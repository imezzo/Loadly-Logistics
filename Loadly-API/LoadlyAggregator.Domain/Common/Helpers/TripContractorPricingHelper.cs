using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractorPricingHelper
{
    public const int AmountScale = 2;

    public static bool RequiresFormulaValidation(ContractConsumptionTypes consumptionType) =>
        consumptionType is ContractConsumptionTypes.CARGO_WEIGHTS
            or ContractConsumptionTypes.DESTINATION_FIXED_PRICE;

    public static bool RequiresContractorPrice(Contract contract) =>
        contract.ContractType == ContractType.OnDemand
        && contract.ConsumptionType is ContractConsumptionTypes.SPOT_PRICING
            or ContractConsumptionTypes.CARGO_WEIGHTS
            or ContractConsumptionTypes.DESTINATION_FIXED_PRICE
            or ContractConsumptionTypes.CARGO_PRICE_COMMISSION;

    public static bool AmountsMatch(decimal submitted, decimal expected) =>
        Math.Round(submitted, AmountScale, MidpointRounding.AwayFromZero)
        == Math.Round(expected, AmountScale, MidpointRounding.AwayFromZero);

    public static decimal? CalculateExpected(
        Contract contract,
        decimal? actualCargoWeightTons,
        ContractVehicleType? contractVehicleType,
        ContractDestination? destination,
        long? contractSlotId)
    {
        if (contract.ContractType != ContractType.OnDemand)
            return null;

        return contract.ConsumptionType switch
        {
            ContractConsumptionTypes.DESTINATION_FIXED_PRICE => destination?.ContractorFixedTripPrice,
            ContractConsumptionTypes.CARGO_WEIGHTS => CalculateCargoWeightsExpected(
                contract,
                actualCargoWeightTons,
                contractVehicleType,
                destination,
                contractSlotId),
            _ => null
        };
    }

    public static ContractorCargoWeightPriceBreakdown? CalculateCargoWeightsBreakdown(
        Contract contract,
        decimal? actualCargoWeightTons,
        ContractVehicleType? contractVehicleType,
        ContractDestination? destination,
        long? contractSlotId)
    {
        if (contract.ConsumptionType != ContractConsumptionTypes.CARGO_WEIGHTS
            || contractVehicleType is null
            || destination is null)
        {
            return null;
        }

        var pricing = ResolvePricingTerms(contract, contractVehicleType, contractSlotId);
        var actualWeight = actualCargoWeightTons ?? 0m;
        var minBillable = pricing.MinBillableWeightTons ?? 0m;
        var billableTons = Math.Max(actualWeight, minBillable);
        var pricePerTon = destination.ContractorPricePerTon ?? 0m;
        var baseCost = billableTons * pricePerTon;
        var overweightCost = CalculateOverweightCost(actualWeight, pricing);

        return new ContractorCargoWeightPriceBreakdown(
            actualWeight,
            minBillable,
            billableTons,
            pricePerTon,
            baseCost,
            pricing.MaxPayloadTons,
            pricing.ContractorOverWeightPricePerTon,
            overweightCost,
            baseCost + overweightCost);
    }

    private static decimal? CalculateCargoWeightsExpected(
        Contract contract,
        decimal? actualCargoWeightTons,
        ContractVehicleType? contractVehicleType,
        ContractDestination? destination,
        long? contractSlotId) =>
        CalculateCargoWeightsBreakdown(contract, actualCargoWeightTons, contractVehicleType, destination, contractSlotId)
            ?.Total;

    private static decimal CalculateOverweightCost(decimal actualWeightTons, ContractTripPricingTerms pricing)
    {
        var ratePerTon = pricing.ContractorOverWeightPricePerTon ?? 0m;
        var maxPayloadTons = pricing.MaxPayloadTons ?? 0m;

        if (ratePerTon <= 0m || maxPayloadTons <= 0m || actualWeightTons <= maxPayloadTons)
            return 0m;

        return (actualWeightTons - maxPayloadTons) * ratePerTon;
    }

    private static ContractTripPricingTerms ResolvePricingTerms(
        Contract contract,
        ContractVehicleType contractVehicleType,
        long? contractSlotId)
    {
        if (TripContractSlotHelper.ShouldDeriveContractSlot(contract) && contractSlotId is long slotId)
        {
            var slot = contract.ContractSlots.FirstOrDefault(s => s.Id == slotId);
            if (slot is not null)
                return ContractTripPricingTerms.FromSlot(slot);
        }

        return ContractTripPricingTerms.FromVehicleType(contractVehicleType);
    }
}

public readonly record struct ContractorCargoWeightPriceBreakdown(
    decimal ActualCargoWeightTons,
    decimal MinBillableWeightTons,
    decimal BillableTons,
    decimal ContractorPricePerTon,
    decimal BaseCost,
    decimal? MaxPayloadTons,
    decimal? ContractorOverWeightPricePerTon,
    decimal OverweightCost,
    decimal Total);
