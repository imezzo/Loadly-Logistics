using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;

public class UpdateVehicleBrandModelAction
{
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IVehicleBrandRepository _vehicleBrandRepository;

    public UpdateVehicleBrandModelAction(
        IVehicleBrandModelRepository vehicleBrandModelRepository,
        IVehicleBrandRepository vehicleBrandRepository)
    {
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _vehicleBrandRepository = vehicleBrandRepository;
    }

    public async Task ExecuteAsync(UpdateVehicleBrandModelDto dto)
    {
        var vehicleBrandModel = await _vehicleBrandModelRepository.GetByIdOrThrowAsync(dto.Id);
        await _vehicleBrandRepository.GetByIdOrThrowAsync(dto.VehicleBrandId);

        vehicleBrandModel.NameAr = dto.NameAr;
        vehicleBrandModel.NameEn = dto.NameEn;
        vehicleBrandModel.VehicleBrandId = dto.VehicleBrandId;
        await _vehicleBrandModelRepository.UpdateAsync(vehicleBrandModel);
        await _vehicleBrandModelRepository.SaveChangesAsync();
    }
}
