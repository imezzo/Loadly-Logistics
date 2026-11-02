using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Containers;

public class ListContainersAction
{
    private readonly IContainerRepository _containerRepository;
    private readonly IMapper _mapper;

    public ListContainersAction(IContainerRepository containerRepository, IMapper mapper)
    {
        _containerRepository = containerRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ContainerDto>> ExecuteAsync(ListContainersDto dto)
    {
        (List<Common.Entities.Container> containers, int totalCount) = await _containerRepository.ListAsync(
            dto.Search,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<ContainerDto>
        {
            Data = _mapper.Map<List<ContainerDto>>(containers),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
