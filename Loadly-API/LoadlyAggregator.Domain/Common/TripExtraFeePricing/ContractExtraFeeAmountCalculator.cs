using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing.Strategies;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;


internal static class ContractExtraFeeAmountCalculator
{
    private sealed record Registration(
        ExtraFeeTypes ExtraFeeType,
        TripChargeType TripChargeType,
        IExtraFeeCalculator Calculator);

    private static readonly Registration[] Registrations =
    [
        // done
        new(ExtraFeeTypes.TollGate, TripChargeType.TollGate, new TollGateTripExtraFeeCalculator()),
        // done (virtual TollGateWeight row when missing — see ContractExtraFeeEffectiveList)
        new(ExtraFeeTypes.TollGateWeight, TripChargeType.TollGateWeight, new TollGateWeightTripExtraFeeCalculator()),
        // not-yet
        new(ExtraFeeTypes.OverWeight, TripChargeType.OverWeight, new OverWeightTripExtraFeeCalculator()),
        // done
        new(ExtraFeeTypes.HelpersFee, TripChargeType.HelpersFee, new HelpersTripExtraFeeCalculator()),
        // done
        new(ExtraFeeTypes.WaitingTime, TripChargeType.WaitingTime, new WaitingTimeTripExtraFeeCalculator()),
        // needs review
        new(ExtraFeeTypes.LoadingUnloading, TripChargeType.LoadingUnloading, new LoadingUnloadingTripExtraFeeCalculator()),
        // done
        new(ExtraFeeTypes.MultiStop, TripChargeType.MultiStop, new MultiStopTripExtraFeeCalculator()),
        // in-progress
        new(ExtraFeeTypes.IncompleteReturn, TripChargeType.IncompleteReturn, new IncompleteReturnTripExtraFeeCalculator()),
        // done
        new(ExtraFeeTypes.Fuel, TripChargeType.Fuel, new FuelTripExtraFeeCalculator()),
        // done
        new(ExtraFeeTypes.OverNight, TripChargeType.OverNight, new OverNightTripExtraFeeCalculator())
    ];

    private static readonly IReadOnlyDictionary<ExtraFeeTypes, Registration> ByExtraFeeType =
        Registrations.ToDictionary(r => r.ExtraFeeType);

    static ContractExtraFeeAmountCalculator()
    {
        foreach (ExtraFeeTypes feeType in Enum.GetValues<ExtraFeeTypes>())
        {
            if (!ByExtraFeeType.ContainsKey(feeType))
            {
                throw new InvalidOperationException(
                    $"Contract extra-fee pricing registry is missing ExtraFeeTypes.{feeType}. Add it to {nameof(Registrations)}.");
            }
        }
    }

    internal static decimal CalculateAmount(ContractExtraFee extraFee, TripExtraFeeInputs inputs)
    {
        var extraFeeType = ToExtraFeeType(extraFee.ExtraFeeTypeId);
        if (!ByExtraFeeType.TryGetValue(extraFeeType, out var registration))
        {
            throw new LogicalException(
                $"Unknown contract extra fee type {extraFeeType} (contract extra fee id {extraFee.Id}); cannot calculate amount.");
        }

        return registration.Calculator.Calculate(extraFee, inputs);
    }

    internal static TripChargeType MapToTripChargeType(int extraFeeTypeId)
    {
        var extraFeeType = ToExtraFeeType(extraFeeTypeId);
        if (!ByExtraFeeType.TryGetValue(extraFeeType, out var registration))
        {
            throw new LogicalException("extra_fees.unknown_id_cannot_map_trip_charge", extraFeeTypeId);
        }

        return registration.TripChargeType;
    }

    private static ExtraFeeTypes ToExtraFeeType(int extraFeeTypeId)
    {
        if (!Enum.IsDefined(typeof(ExtraFeeTypes), extraFeeTypeId))
        {
            throw new LogicalException("extra_fees.unknown_extra_fee_types_id", extraFeeTypeId);
        }

        return (ExtraFeeTypes)extraFeeTypeId;
    }
}
