using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public class DeleteVehicleTypeAction
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;

    public DeleteVehicleTypeAction(IVehicleTypeRepository vehicleTypeRepository)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleTypeDto dto)
    {
        var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.Id);
        if (vehicleType.VehicleTypeTags.Count > 0)
        {
            _vehicleTypeRepository.RemoveVehicleTypeTags(vehicleType.VehicleTypeTags);
        }

        _vehicleTypeRepository.Remove(vehicleType);
        await _vehicleTypeRepository.SaveChangesAsync();
    }
}
