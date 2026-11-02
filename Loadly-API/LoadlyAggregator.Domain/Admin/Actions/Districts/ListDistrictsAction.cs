using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Districts;

public class ListDistrictsAction
{
    private readonly IDistrictRepository _districtRepository;
    private readonly IMapper _mapper;

    public ListDistrictsAction(IDistrictRepository districtRepository, IMapper mapper)
    {
        _districtRepository = districtRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<DistrictDto>> ExecuteAsync(ListDistrictsDto dto)
    {
        var (districts, totalCount) = await _districtRepository.ListAsync(
            dto.Search,
            dto.CityId,
            dto.Page,
            dto.PageSize);
        return new ListingDto<DistrictDto>
        {
            Data = _mapper.Map<List<DistrictDto>>(districts),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
