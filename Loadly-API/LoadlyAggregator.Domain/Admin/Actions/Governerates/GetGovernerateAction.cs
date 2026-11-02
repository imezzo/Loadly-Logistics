using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Governerates;

public class GetGovernerateAction
{
    private readonly IGovernerateRepository _governerateRepository;
    private readonly IMapper _mapper;

    public GetGovernerateAction(IGovernerateRepository governerateRepository, IMapper mapper)
    {
        _governerateRepository = governerateRepository;
        _mapper = mapper;
    }

    public async Task<GovernerateDto> ExecuteAsync(GetGovernerateDto dto)
    {
        var governerate = await _governerateRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<GovernerateDto>(governerate);
    }
}
