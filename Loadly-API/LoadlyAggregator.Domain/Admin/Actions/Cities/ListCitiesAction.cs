using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Cities;

public class ListCitiesAction
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public ListCitiesAction(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<CityDto>> ExecuteAsync(ListCitiesDto dto)
    {
        var (cities, totalCount) = await _cityRepository.ListAsync(
            dto.Search,
            dto.GovernerateId,
            dto.Page,
            dto.PageSize);
        return new ListingDto<CityDto>
        {
            Data = _mapper.Map<List<CityDto>>(cities),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
