using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Cities;

public class GetCityAction
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public GetCityAction(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<CityDto> ExecuteAsync(GetCityDto dto)
    {
        var city = await _cityRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<CityDto>(city);
    }
}
