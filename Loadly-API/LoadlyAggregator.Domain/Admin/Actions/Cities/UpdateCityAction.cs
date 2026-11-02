using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Cities;

public class UpdateCityAction
{
    private readonly ICityRepository _cityRepository;

    public UpdateCityAction(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task ExecuteAsync(UpdateCityDto dto)
    {
        var city = await _cityRepository.GetByIdOrThrowAsync(dto.Id);
        city.NameEn = dto.NameEn;
        city.NameAr = dto.NameAr;
        city.GovernerateId = dto.GovernerateId;
        await _cityRepository.UpdateAsync(city);
        await _cityRepository.SaveChangesAsync();
    }
}
