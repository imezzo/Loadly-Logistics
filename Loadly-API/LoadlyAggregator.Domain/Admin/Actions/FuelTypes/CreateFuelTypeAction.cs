using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.FuelTypes;

public class CreateFuelTypeAction
{
    private readonly IFuelTypeRepository _fuelTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateFuelTypeAction(
        IFuelTypeRepository fuelTypeRepository,
        ICurrentUserService currentUserService)
    {
        _fuelTypeRepository = fuelTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateFuelTypeDto dto)
    {
        await _fuelTypeRepository.AddAsync(new FuelType
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            UnitPrice = dto.UnitPrice,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _fuelTypeRepository.SaveChangesAsync();
    }
}
