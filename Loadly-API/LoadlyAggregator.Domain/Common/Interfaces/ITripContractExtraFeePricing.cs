using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;

namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Contract extra-fee pricing for trips: builds per-trip inputs and computes amounts / charge types.
/// </summary>
public interface ITripContractExtraFeePricing
{
    TripExtraFeeInputs ForTrip(
        Trip trip,
        decimal baseCharge,
        ContractTripPricingTerms pricing,
        string contractReferenceNumber,
        IReadOnlyList<ContractExtraFee> contractExtraFees);

    decimal CalculateAmount(ContractExtraFee extraFee, TripExtraFeeInputs inputs);

    TripChargeType MapToTripChargeType(int extraFeeTypeId);
}
