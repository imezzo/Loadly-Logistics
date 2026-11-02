using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;

public class DeleteVehicleCapacityUnitAction
{
    private readonly IVehicleCapacityUnitRepository _vehicleCapacityUnitRepository;

    public DeleteVehicleCapacityUnitAction(IVehicleCapacityUnitRepository vehicleCapacityUnitRepository)
    {
        _vehicleCapacityUnitRepository = vehicleCapacityUnitRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleCapacityUnitDto dto)
    {
        var vehicleCapacityUnit = await _vehicleCapacityUnitRepository.GetByIdOrThrowAsync(dto.Id);
        _vehicleCapacityUnitRepository.Remove(vehicleCapacityUnit);
        await _vehicleCapacityUnitRepository.SaveChangesAsync();
    }
}
