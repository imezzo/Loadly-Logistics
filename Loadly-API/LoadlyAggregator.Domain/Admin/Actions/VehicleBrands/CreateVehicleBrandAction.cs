using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;

public class CreateVehicleBrandAction
{
    private readonly IVehicleBrandRepository _vehicleBrandRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleBrandAction(
        IVehicleBrandRepository vehicleBrandRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleBrandRepository = vehicleBrandRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleBrandDto dto)
    {
        var vehicleBrand = new Common.Entities.VehicleBrand
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Logo = dto.Logo,
            AddedBy = _currentUserService.GetCurrentUserId()
        };
        await _vehicleBrandRepository.AddAsync(vehicleBrand);
        await _vehicleBrandRepository.SaveChangesAsync();
    }
}
