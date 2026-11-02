using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Ports;

public class ListPortsAction
{
    private readonly IPortRepository _portRepository;
    private readonly IMapper _mapper;

    public ListPortsAction(IPortRepository portRepository, IMapper mapper)
    {
        _portRepository = portRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<PortDto>> ExecuteAsync(ListPortsDto dto)
    {
        (List<Common.Entities.Port> ports, int totalCount) = await _portRepository.ListAsync(
            dto.Search,
            dto.Type,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<PortDto>
        {
            Data = _mapper.Map<List<PortDto>>(ports),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
