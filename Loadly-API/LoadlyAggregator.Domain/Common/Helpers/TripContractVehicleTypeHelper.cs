using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractVehicleTypeHelper
{
    public readonly record struct ContractVehicleTypeProfile(bool UsesHeadTypesOnly, bool HasTailTypes)
    {
        public static ContractVehicleTypeProfile From(IEnumerable<ContractVehicleType> contractVehicleTypes)
        {
            var types = contractVehicleTypes as ICollection<ContractVehicleType> ?? contractVehicleTypes.ToList();
            return new(
                UsesHeadTypesOnly: types.Count > 0
                    && types.All(cvt => cvt.VehicleType?.VehicleUnitType == VehicleUnitType.Head),
                HasTailTypes: types.Any(cvt => cvt.VehicleType?.VehicleUnitType == VehicleUnitType.Tail));
        }
    }

    public static ContractVehicleTypeProfile GetContractProfile(IEnumerable<ContractVehicleType> contractVehicleTypes) =>
        ContractVehicleTypeProfile.From(contractVehicleTypes);

    /// <summary>
    /// Live/import paths: explicit type, else request tail, else currently attached tail on head.
    /// </summary>
    public static bool TryResolveVehicleTypeId(
        Vehicle headVehicle,
        long? explicitVehicleTypeId,
        Vehicle? requestTail,
        out long resolvedVehicleTypeId)
    {
        if (TryTakeExplicitVehicleTypeId(explicitVehicleTypeId, out resolvedVehicleTypeId))
            return true;

        if (requestTail != null)
        {
            resolvedVehicleTypeId = requestTail.VehicleTypeId;
            return true;
        }

        return TryResolveFromCurrentlyAttachedTail(headVehicle, out resolvedVehicleTypeId);
    }

    /// <summary>
    /// Historical backfill: never infers trailer from current DB attachment.
    /// </summary>
    public static bool TryResolveHistoricalVehicleTypeId(
        Vehicle vehicle,
        long? explicitVehicleTypeId,
        Vehicle? requestTail,
        ContractVehicleTypeProfile contract,
        out long resolvedVehicleTypeId)
    {
        if (TryTakeExplicitVehicleTypeId(explicitVehicleTypeId, out resolvedVehicleTypeId))
            return true;

        return UnitTypeOf(vehicle) switch
        {
            VehicleUnitType.RIGID => SetResolved(vehicle.VehicleTypeId, out resolvedVehicleTypeId),
            VehicleUnitType.Head => TryResolveHistoricalHeadUnit(
                vehicle, requestTail, contract, out resolvedVehicleTypeId),
            _ => SetResolved(vehicle.VehicleTypeId, out resolvedVehicleTypeId),
        };
    }

    /// <summary>
    /// Single attached tail (or same-type tails) on head; ambiguous when multiple tail types are attached.
    /// </summary>
    public static bool TryResolveFromCurrentlyAttachedTail(Vehicle headVehicle, out long vehicleTypeId)
    {
        if (headVehicle.TailVehicles is not { Count: > 0 } attachedTails)
        {
            vehicleTypeId = headVehicle.VehicleTypeId;
            return true;
        }

        var distinctTailTypeIds = attachedTails
            .Select(t => t.VehicleTypeId)
            .Distinct()
            .ToList();

        if (distinctTailTypeIds.Count > 1)
        {
            vehicleTypeId = 0;
            return false;
        }

        vehicleTypeId = distinctTailTypeIds[0];
        return true;
    }

    private static bool TryResolveHistoricalHeadUnit(
        Vehicle headVehicle,
        Vehicle? requestTail,
        ContractVehicleTypeProfile contract,
        out long resolvedVehicleTypeId)
    {
        if (contract.UsesHeadTypesOnly)
            return SetResolved(headVehicle.VehicleTypeId, out resolvedVehicleTypeId);

        if (requestTail != null)
            return SetResolved(requestTail.VehicleTypeId, out resolvedVehicleTypeId);

        if (contract.HasTailTypes)
        {
            resolvedVehicleTypeId = 0;
            return false;
        }

        return SetResolved(headVehicle.VehicleTypeId, out resolvedVehicleTypeId);
    }

    private static bool TryTakeExplicitVehicleTypeId(long? explicitVehicleTypeId, out long resolvedVehicleTypeId)
    {
        if (explicitVehicleTypeId is > 0)
        {
            resolvedVehicleTypeId = explicitVehicleTypeId.Value;
            return true;
        }

        resolvedVehicleTypeId = 0;
        return false;
    }

    private static VehicleUnitType? UnitTypeOf(Vehicle vehicle) =>
        vehicle.VehicleType?.VehicleUnitType;

    private static bool SetResolved(long vehicleTypeId, out long resolvedVehicleTypeId)
    {
        resolvedVehicleTypeId = vehicleTypeId;
        return true;
    }
}
