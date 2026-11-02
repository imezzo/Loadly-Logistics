using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Ports;

public class GetPortAction
{
    private readonly IPortRepository _portRepository;
    private readonly IMapper _mapper;

    public GetPortAction(IPortRepository portRepository, IMapper mapper)
    {
        _portRepository = portRepository;
        _mapper = mapper;
    }

    public async Task<PortDto> ExecuteAsync(GetPortDto dto)
    {
        Common.Entities.Port port = await _portRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<PortDto>(port);
    }
}
