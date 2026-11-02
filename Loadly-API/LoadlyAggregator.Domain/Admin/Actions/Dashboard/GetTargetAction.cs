using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetTargetAction
{
    private readonly ITargetRepository _targetRepository;
    private readonly IMapper _mapper;

    public GetTargetAction(ITargetRepository targetRepository, IMapper mapper)
    {
        _targetRepository = targetRepository;
        _mapper = mapper;
    }

    public async Task<TargetDto> ExecuteAsync(GetTargetDto dto)
    {
        var target = await _targetRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<TargetDto>(target);
    }
}
