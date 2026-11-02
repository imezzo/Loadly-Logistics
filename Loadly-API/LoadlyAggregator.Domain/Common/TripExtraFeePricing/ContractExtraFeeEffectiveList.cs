using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

/// <summary>
/// Resolves contract extra fees effective for trip charge calculation (e.g. virtual rows when config is implied).
/// </summary>
internal static class ContractExtraFeeEffectiveList
{
    /// <summary>
    /// No TollGateWeight row on contract: bill weight tolls using TollGate's option/value (same calculator as own weight fee).
    /// </summary>
    internal static IReadOnlyList<ContractExtraFee> WithTollGateWeightFallback(
        IReadOnlyList<ContractExtraFee> fees,
        TripExtraFeeInputs inputs)
    {
        if (inputs.TollgatesWeightTotalAmount is null)
            return fees;

        if (fees.Any(f => f.ExtraFeeTypeId == (int)ExtraFeeTypes.TollGateWeight))
            return fees;

        if (fees.FirstOrDefault(f => f.ExtraFeeTypeId == (int)ExtraFeeTypes.TollGate) is not { } tollGate)
            return fees;

        return fees.Append(TollGateWeightFrom(tollGate)).ToList();
    }

    private static ContractExtraFee TollGateWeightFrom(ContractExtraFee tollGate) =>
        new()
        {
            ExtraFeeTypeId = (int)ExtraFeeTypes.TollGateWeight,
            CalculationOption = tollGate.CalculationOption,
            Value = tollGate.Value,
            AllowOverride = tollGate.AllowOverride
        };
}
