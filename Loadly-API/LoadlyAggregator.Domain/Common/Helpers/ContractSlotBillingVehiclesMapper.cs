using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractSlotBillingVehiclesMapper
{
    public static IReadOnlyDictionary<long, VehicleRelationDto> MapVehicles(
        IReadOnlyList<ContractSlot> billedSlots)
    {
        var map = new Dictionary<long, VehicleRelationDto>();
        foreach (var slot in billedSlots)
        {
            var vehicleType = ResolveVehicleType(slot);
            var vehicle = MapVehicle(slot.Vehicle, vehicleType);
            if (vehicle is not null)
                map[slot.Id] = vehicle;
        }

        return map;
    }

    public static void ApplyVehicles(
        CalculateIncludedContractSlotChargesResultDto result,
        IReadOnlyList<ContractSlot> billedSlots) =>
        result.Vehicles = MapVehicles(billedSlots);

    public static void ApplyVehicles(
        CalculateIncludedContractSlotChargesResultDto result,
        Contract contract,
        DateOnly periodStart,
        DateOnly periodEnd,
        long? contractSlotId = null)
    {
        var billedSlots = IncludedContractSlotChargesComputer.ResolveTargetSlots(
            contract,
            contractSlotId,
            periodStart,
            periodEnd);
        ApplyVehicles(result, billedSlots);
        ContractSlotSeatRootMapper.Apply(result, contract.ContractSlots.ToList());
    }

    private static VehicleTypeRelationDto? ResolveVehicleType(ContractSlot slot)
    {
        if (slot.VehicleType is not null)
            return MapVehicleType(slot.VehicleType);

        if (slot.Vehicle?.VehicleType is not null)
            return MapVehicleType(slot.Vehicle.VehicleType);

        return null;
    }

    private static VehicleRelationDto? MapVehicle(Vehicle? vehicle, VehicleTypeRelationDto? fallbackType)
    {
        if (vehicle is null)
            return null;

        return new VehicleRelationDto
        {
            Id = vehicle.Id,
            VehicleTypeId = vehicle.VehicleTypeId,
            PlateNo = vehicle.PlateNo ?? string.Empty,
            PlateLetters = vehicle.PlateLetters ?? string.Empty,
            ChassisNumber = vehicle.VinNo,
            BrandName = vehicle.VehicleBrandModel?.VehicleBrand?.NameEn,
            ModelName = vehicle.VehicleBrandModel?.NameEn,
            Year = vehicle.MakeYear,
            SectorId = vehicle.SectorId,
            VehicleType = vehicle.VehicleType is not null
                ? MapVehicleType(vehicle.VehicleType)
                : fallbackType,
            Driver = vehicle.Driver is null
                ? null
                : new DriverRelationDto
                {
                    Id = vehicle.Driver.Id,
                    FullName = vehicle.Driver.FullName,
                    FullNameEn = vehicle.Driver.FullName,
                    Phone = vehicle.Driver.Phone,
                    Email = vehicle.Driver.Email
                }
        };
    }

    private static VehicleTypeRelationDto MapVehicleType(VehicleType vehicleType) =>
        new()
        {
            Id = vehicleType.Id,
            NameEn = vehicleType.NameEn,
            NameAr = vehicleType.NameAr,
            Category = vehicleType.Category,
            VehicleUnitType = vehicleType.VehicleUnitType
        };
}
