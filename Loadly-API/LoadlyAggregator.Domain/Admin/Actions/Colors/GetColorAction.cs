using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Colors;

public class GetColorAction
{
    private readonly IColorRepository _colorRepository;
    private readonly IMapper _mapper;

    public GetColorAction(IColorRepository colorRepository, IMapper mapper)
    {
        _colorRepository = colorRepository;
        _mapper = mapper;
    }

    public async Task<ColorDto> ExecuteAsync(GetColorDto dto)
    {
        var color = await _colorRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ColorDto>(color);
    }
}
