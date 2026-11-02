using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;

public class GetCommunicationMethodAction
{
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly IMapper _mapper;

    public GetCommunicationMethodAction(
        ICommunicationMethodRepository communicationMethodRepository,
        IMapper mapper)
    {
        _communicationMethodRepository = communicationMethodRepository;
        _mapper = mapper;
    }

    public async Task<CommunicationMethodDto> ExecuteAsync(GetCommunicationMethodDto dto)
    {
        var communicationMethod = await _communicationMethodRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<CommunicationMethodDto>(communicationMethod);
    }
}
