using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;

public class UpdateVehicleCapacityUnitAction
{
    private readonly IVehicleCapacityUnitRepository _vehicleCapacityUnitRepository;

    public UpdateVehicleCapacityUnitAction(IVehicleCapacityUnitRepository vehicleCapacityUnitRepository)
    {
        _vehicleCapacityUnitRepository = vehicleCapacityUnitRepository;
    }

    public async Task ExecuteAsync(UpdateVehicleCapacityUnitDto dto)
    {
        var vehicleCapacityUnit = await _vehicleCapacityUnitRepository.GetByIdOrThrowAsync(dto.Id);

        vehicleCapacityUnit.CapacityTypeEn = dto.CapacityTypeEn;
        vehicleCapacityUnit.CapacityTypeAr = dto.CapacityTypeAr;
        await _vehicleCapacityUnitRepository.UpdateAsync(vehicleCapacityUnit);
        await _vehicleCapacityUnitRepository.SaveChangesAsync();
    }
}
