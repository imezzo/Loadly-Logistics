using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class DeleteVehicleAction
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleAction(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdOrThrowAsync(dto.Id);
        _vehicleRepository.Remove(vehicle);
        await _vehicleRepository.SaveChangesAsync();
    }
}
