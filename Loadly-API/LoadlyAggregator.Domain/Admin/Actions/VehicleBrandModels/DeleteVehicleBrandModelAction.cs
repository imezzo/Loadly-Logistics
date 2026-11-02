using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;

public class DeleteVehicleBrandModelAction
{
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;

    public DeleteVehicleBrandModelAction(IVehicleBrandModelRepository vehicleBrandModelRepository)
    {
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleBrandModelDto dto)
    {
        var vehicleBrandModel = await _vehicleBrandModelRepository.GetByIdOrThrowAsync(dto.Id);
        _vehicleBrandModelRepository.Remove(vehicleBrandModel);
        await _vehicleBrandModelRepository.SaveChangesAsync();
    }
}
