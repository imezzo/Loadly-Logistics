using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;

public class ListIndustryTypesAction
{
    private readonly IIndustryTypeRepository _industryTypeRepository;
    private readonly IMapper _mapper;

    public ListIndustryTypesAction(IIndustryTypeRepository industryTypeRepository, IMapper mapper)
    {
        _industryTypeRepository = industryTypeRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<IndustryTypeDto>> ExecuteAsync(ListIndustryTypesDto dto)
    {
        var (industryTypes, totalCount) = await _industryTypeRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<IndustryTypeDto>
        {
            Data = _mapper.Map<List<IndustryTypeDto>>(industryTypes),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
