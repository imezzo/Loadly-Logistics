using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.FuelTypes;

public class UpdateFuelTypeAction
{
    private readonly IFuelTypeRepository _fuelTypeRepository;

    public UpdateFuelTypeAction(IFuelTypeRepository fuelTypeRepository)
    {
        _fuelTypeRepository = fuelTypeRepository;
    }

    public async Task ExecuteAsync(UpdateFuelTypeDto dto)
    {
        var fuelType = await _fuelTypeRepository.GetByIdOrThrowAsync(dto.Id);
        fuelType.NameAr = dto.NameAr;
        fuelType.NameEn = dto.NameEn;
        fuelType.UnitPrice = dto.UnitPrice;
        await _fuelTypeRepository.UpdateAsync(fuelType);
        await _fuelTypeRepository.SaveChangesAsync();
    }
}
