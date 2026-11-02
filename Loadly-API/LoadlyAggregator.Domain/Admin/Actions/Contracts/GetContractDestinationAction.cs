using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractDestinationAction
{
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IMapper _mapper;

    public GetContractDestinationAction(
        IContractDestinationRepository contractDestinationRepository,
        IMapper mapper)
    {
        _contractDestinationRepository = contractDestinationRepository;
        _mapper = mapper;
    }

    public async Task<ContractDestinationDto> ExecuteAsync(GetContractDestinationDto dto)
    {
        var destination = await _contractDestinationRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ContractDestinationDto>(destination);
    }
}
