using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;

public class UpdateVehicleBrandAction
{
    private readonly IVehicleBrandRepository _vehicleBrandRepository;

    public UpdateVehicleBrandAction(IVehicleBrandRepository vehicleBrandRepository)
    {
        _vehicleBrandRepository = vehicleBrandRepository;
    }

    public async Task ExecuteAsync(UpdateVehicleBrandDto dto)
    {
        var vehicleBrand = await _vehicleBrandRepository.GetByIdOrThrowAsync(dto.Id);

        vehicleBrand.NameAr = dto.NameAr;
        vehicleBrand.NameEn = dto.NameEn;
        vehicleBrand.Logo = dto.Logo;
        await _vehicleBrandRepository.UpdateAsync(vehicleBrand);
        await _vehicleBrandRepository.SaveChangesAsync();
    }
}
