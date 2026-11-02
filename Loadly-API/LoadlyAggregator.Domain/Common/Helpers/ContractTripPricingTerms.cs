using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public readonly record struct ContractTripPricingTerms(
    decimal? DailyValue,
    decimal? MaxKms,
    decimal? OverConsumption,
    decimal? MinBillableWeightTons,
    decimal? MaxPayloadTons,
    decimal? OverWeightPricePerTon,
    decimal? ContractorOverWeightPricePerTon,
    decimal? ConsumptionRate)
{
    public static ContractTripPricingTerms FromVehicleType(ContractVehicleType source) =>
        new(
            source.DailyValue,
            source.MaxKms,
            source.OverConsumption,
            source.MinBillableWeightTons,
            source.MaxPayloadTons,
            source.OverWeightPricePerTon,
            source.ContractorOverWeightPricePerTon,
            source.ConsumptionRate);

    public static ContractTripPricingTerms FromSlot(ContractSlot source) =>
        new(
            source.DailyValue,
            source.MaxKms,
            source.OverConsumption,
            source.MinBillableWeightTons,
            source.MaxPayloadTons,
            source.OverWeightPricePerTon,
            source.ContractorOverWeightPricePerTon,
            source.ConsumptionRate);
}
