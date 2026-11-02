using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <inheritdoc />
public sealed class TripContractExtraFeePricingService : ITripContractExtraFeePricing
{
    /// <inheritdoc />
    public TripExtraFeeInputs ForTrip(
        Trip trip,
        decimal baseCharge,
        ContractTripPricingTerms pricing,
        string contractReferenceNumber,
        IReadOnlyList<ContractExtraFee> contractExtraFees) =>
        TripExtraFeeInputFactory.CreateForTrip(
            trip,
            baseCharge,
            pricing,
            contractReferenceNumber,
            contractExtraFees);

    /// <inheritdoc />
    public decimal CalculateAmount(ContractExtraFee extraFee, TripExtraFeeInputs inputs) =>
        ContractExtraFeeAmountCalculator.CalculateAmount(extraFee, inputs);

    /// <inheritdoc />
    public TripChargeType MapToTripChargeType(int extraFeeTypeId) =>
        ContractExtraFeeAmountCalculator.MapToTripChargeType(extraFeeTypeId);
}
