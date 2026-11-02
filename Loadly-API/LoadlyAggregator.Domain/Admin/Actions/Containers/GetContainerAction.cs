using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Containers;

public class GetContainerAction
{
    private readonly IContainerRepository _containerRepository;
    private readonly IMapper _mapper;

    public GetContainerAction(IContainerRepository containerRepository, IMapper mapper)
    {
        _containerRepository = containerRepository;
        _mapper = mapper;
    }

    public async Task<ContainerDto> ExecuteAsync(GetContainerDto dto)
    {
        Common.Entities.Container container = await _containerRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ContainerDto>(container);
    }
}
