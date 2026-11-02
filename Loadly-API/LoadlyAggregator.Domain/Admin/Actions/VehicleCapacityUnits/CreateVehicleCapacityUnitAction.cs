using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;

public class CreateVehicleCapacityUnitAction
{
    private readonly IVehicleCapacityUnitRepository _vehicleCapacityUnitRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleCapacityUnitAction(
        IVehicleCapacityUnitRepository vehicleCapacityUnitRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleCapacityUnitRepository = vehicleCapacityUnitRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleCapacityUnitDto dto)
    {
        var vehicleCapacityUnit = new Common.Entities.VehicleCapacityUnit
        {
            CapacityTypeEn = dto.CapacityTypeEn,
            CapacityTypeAr = dto.CapacityTypeAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        };
        await _vehicleCapacityUnitRepository.AddAsync(vehicleCapacityUnit);
        await _vehicleCapacityUnitRepository.SaveChangesAsync();
    }
}
