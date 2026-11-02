using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;

public class GetServiceRequirementAction
{
    private readonly IServiceRequirementRepository _serviceRequirementRepository;
    private readonly IMapper _mapper;

    public GetServiceRequirementAction(IServiceRequirementRepository serviceRequirementRepository, IMapper mapper)
    {
        _serviceRequirementRepository = serviceRequirementRepository;
        _mapper = mapper;
    }

    public async Task<ServiceRequirementDto> ExecuteAsync(GetServiceRequirementDto dto)
    {
        var serviceRequirement = await _serviceRequirementRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ServiceRequirementDto>(serviceRequirement);
    }
}
