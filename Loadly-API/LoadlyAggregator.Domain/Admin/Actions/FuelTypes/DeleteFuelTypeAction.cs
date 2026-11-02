using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.FuelTypes;

public class DeleteFuelTypeAction
{
    private readonly IFuelTypeRepository _fuelTypeRepository;

    public DeleteFuelTypeAction(IFuelTypeRepository fuelTypeRepository)
    {
        _fuelTypeRepository = fuelTypeRepository;
    }

    public async Task ExecuteAsync(DeleteFuelTypeDto dto)
    {
        _fuelTypeRepository.Remove(await _fuelTypeRepository.GetByIdOrThrowAsync(dto.Id));
        await _fuelTypeRepository.SaveChangesAsync();
    }
}
