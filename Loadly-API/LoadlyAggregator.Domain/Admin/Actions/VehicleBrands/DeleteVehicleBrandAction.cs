using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;

public class DeleteVehicleBrandAction
{
    private readonly IVehicleBrandRepository _vehicleBrandRepository;

    public DeleteVehicleBrandAction(IVehicleBrandRepository vehicleBrandRepository)
    {
        _vehicleBrandRepository = vehicleBrandRepository;
    }

    public async Task ExecuteAsync(DeleteVehicleBrandDto dto)
    {
        var vehicleBrand = await _vehicleBrandRepository.GetByIdOrThrowAsync(dto.Id);
        _vehicleBrandRepository.Remove(vehicleBrand);
        await _vehicleBrandRepository.SaveChangesAsync();
    }
}
