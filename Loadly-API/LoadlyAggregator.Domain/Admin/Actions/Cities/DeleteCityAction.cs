using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Cities;

public class DeleteCityAction
{
    private readonly ICityRepository _cityRepository;

    public DeleteCityAction(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task ExecuteAsync(DeleteCityDto dto)
    {
        _cityRepository.Remove(await _cityRepository.GetByIdOrThrowAsync(dto.Id));
        await _cityRepository.SaveChangesAsync();
    }
}
