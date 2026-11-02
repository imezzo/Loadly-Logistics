using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Contracts;

public class CompanyGetContractDestinationAction
{
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IMapper _mapper;

    public CompanyGetContractDestinationAction(
        IContractDestinationRepository contractDestinationRepository,
        IMapper mapper)
    {
        _contractDestinationRepository = contractDestinationRepository;
        _mapper = mapper;
    }

    public async Task<ContractDestinationDto> ExecuteAsync(CompanyGetContractDestinationDto dto)
    {
        var destination = await _contractDestinationRepository.GetByIdOrThrowAsync(dto.Id);

        if (destination.Contract.CompanyId != dto.CompanyId)
        {
            throw new ForbiddenException("contracts.not_authorized");
        }

        if (destination.Contract.Status != ContractStatus.ACTIVE)
        {
            throw new NotFoundException("contracts.not_found_with_id", dto.Id);
        }

        return _mapper.Map<ContractDestinationDto>(destination);
    }
}
