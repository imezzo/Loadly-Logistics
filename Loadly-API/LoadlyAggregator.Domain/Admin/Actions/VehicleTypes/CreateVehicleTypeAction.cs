using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public class CreateVehicleTypeAction
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleTypeAction(
        IVehicleTypeRepository vehicleTypeRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleTypeDto dto)
    {
        await VehicleTypeTagValidation.ValidateAsync(_vehicleTypeRepository, dto.TagIds);

        var addedBy = _currentUserService.GetCurrentUserId();
        var vehicleType = new Common.Entities.VehicleType
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Category = dto.Category,
            Status = dto.Status,
            VehicleUnitType = dto.VehicleUnitType,
            AddedBy = addedBy,
            VehicleTypeTags = dto.TagIds.Distinct()
                .Select(tagId => new VehicleTypeTag
                {
                    TagId = tagId,
                    AddedBy = addedBy
                })
                .ToList()
        };
        await _vehicleTypeRepository.AddAsync(vehicleType);
        await _vehicleTypeRepository.SaveChangesAsync();
    }
}
