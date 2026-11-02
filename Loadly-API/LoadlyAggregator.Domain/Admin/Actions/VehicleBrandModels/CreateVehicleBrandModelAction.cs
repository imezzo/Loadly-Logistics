using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;

public class CreateVehicleBrandModelAction
{
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IVehicleBrandRepository _vehicleBrandRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleBrandModelAction(
        IVehicleBrandModelRepository vehicleBrandModelRepository,
        IVehicleBrandRepository vehicleBrandRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _vehicleBrandRepository = vehicleBrandRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleBrandModelDto dto)
    {
        await _vehicleBrandRepository.GetByIdOrThrowAsync(dto.VehicleBrandId);

        var vehicleBrandModel = new Common.Entities.VehicleBrandModel
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            VehicleBrandId = dto.VehicleBrandId,
            AddedBy = _currentUserService.GetCurrentUserId()
        };
        await _vehicleBrandModelRepository.AddAsync(vehicleBrandModel);
        await _vehicleBrandModelRepository.SaveChangesAsync();
    }
}
