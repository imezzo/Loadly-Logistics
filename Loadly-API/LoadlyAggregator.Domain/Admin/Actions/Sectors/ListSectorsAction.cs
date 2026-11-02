using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Sectors;

public class ListSectorsAction
{
    private readonly ISectorRepository _sectorRepository;
    private readonly IMapper _mapper;

    public ListSectorsAction(ISectorRepository sectorRepository, IMapper mapper)
    {
        _sectorRepository = sectorRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<SectorDto>> ExecuteAsync(ListSectorsDto dto)
    {
        var (sectors, totalCount) = await _sectorRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<SectorDto>
        {
            Data = _mapper.Map<List<SectorDto>>(sectors),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
