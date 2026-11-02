using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Governerates;

public class ListGoverneratesAction
{
    private readonly IGovernerateRepository _governerateRepository;
    private readonly IMapper _mapper;

    public ListGoverneratesAction(IGovernerateRepository governerateRepository, IMapper mapper)
    {
        _governerateRepository = governerateRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<GovernerateDto>> ExecuteAsync(ListGoverneratesDto dto)
    {
        var (governerates, totalCount) = await _governerateRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<GovernerateDto>
        {
            Data = _mapper.Map<List<GovernerateDto>>(governerates),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
