using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Helpers;

/// <summary>
/// Shared reference validation for contract create/update (DRY).
/// </summary>
public static class ContractReferenceValidator
{
    public static async Task EnsureVehicleTypesExistAsync(
        IReadOnlyList<ContractVehicleTypeDto>? vehicleTypes,
        IVehicleTypeRepository vehicleTypeRepository)
    {
        if (vehicleTypes == null) return;
        foreach (var item in vehicleTypes)
        {
            _ = await vehicleTypeRepository.GetByIdAsync(item.VehicleTypeId)
                ?? throw new NotFoundException("vehicle_types.not_found_with_id", item.VehicleTypeId);
        }
    }

    public static async Task EnsureVehicleTypesExistAsync(
        IReadOnlyList<CreateContractVehicleTypeDto>? vehicleTypes,
        IVehicleTypeRepository vehicleTypeRepository)
    {
        if (vehicleTypes == null) return;
        foreach (var item in vehicleTypes)
        {
            _ = await vehicleTypeRepository.GetByIdAsync(item.VehicleTypeId)
                ?? throw new NotFoundException("vehicle_types.not_found_with_id", item.VehicleTypeId);
        }
    }

    public static async Task EnsureSlotReferencesExistAsync(
        IReadOnlyList<ContractSlotItemDto>? slots,
        IVehicleTypeRepository vehicleTypeRepository,
        IVehicleRepository vehicleRepository)
    {
        if (slots == null) return;
        foreach (var item in slots)
        {
            _ = await vehicleTypeRepository.GetByIdAsync(item.VehicleTypeId)
                ?? throw new NotFoundException("vehicle_types.not_found_with_id", item.VehicleTypeId);
            if (item.VehicleId is { } vehicleId)
            {
                _ = await vehicleRepository.GetByIdAsync(vehicleId)
                    ?? throw new NotFoundException("vehicles.not_found_with_id", vehicleId);
            }
        }
    }
}
