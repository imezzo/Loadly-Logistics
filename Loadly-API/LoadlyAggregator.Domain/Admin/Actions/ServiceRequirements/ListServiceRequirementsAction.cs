using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;

public class ListServiceRequirementsAction
{
    private readonly IServiceRequirementRepository _serviceRequirementRepository;
    private readonly IMapper _mapper;

    public ListServiceRequirementsAction(IServiceRequirementRepository serviceRequirementRepository, IMapper mapper)
    {
        _serviceRequirementRepository = serviceRequirementRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ServiceRequirementDto>> ExecuteAsync(ListServiceRequirementsDto dto)
    {
        var (serviceRequirements, totalCount) = await _serviceRequirementRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<ServiceRequirementDto>
        {
            Data = _mapper.Map<List<ServiceRequirementDto>>(serviceRequirements),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
