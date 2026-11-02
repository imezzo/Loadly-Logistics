using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Tags;

public class GetTagAction
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetTagAction(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<TagDto> ExecuteAsync(GetTagDto dto)
    {
        var tag = await _tagRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<TagDto>(tag);
    }
}
