using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Colors;

public class ListColorsAction
{
    private readonly IColorRepository _colorRepository;
    private readonly IMapper _mapper;

    public ListColorsAction(IColorRepository colorRepository, IMapper mapper)
    {
        _colorRepository = colorRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ColorDto>> ExecuteAsync(BasePagedRequest dto)
    {
        var (colors, totalCount) = await _colorRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<ColorDto>
        {
            Data = _mapper.Map<List<ColorDto>>(colors),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
