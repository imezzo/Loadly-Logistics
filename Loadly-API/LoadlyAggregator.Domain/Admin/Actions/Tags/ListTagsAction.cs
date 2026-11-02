using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Tags;

public class ListTagsAction
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public ListTagsAction(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<TagDto>> ExecuteAsync(ListTagsDto dto)
    {
        var (tags, totalCount) = await _tagRepository.ListAsync(
            dto.Search,
            dto.IsPricing,
            dto.Page,
            dto.PageSize);
        return new ListingDto<TagDto>
        {
            Data = _mapper.Map<List<TagDto>>(tags),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
