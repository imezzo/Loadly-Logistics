using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class ListTargetsAction
{
    private readonly ITargetRepository _targetRepository;
    private readonly IMapper _mapper;

    public ListTargetsAction(ITargetRepository targetRepository, IMapper mapper)
    {
        _targetRepository = targetRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<TargetDto>> ExecuteAsync(ListTargetsDto dto)
    {
        var (targets, totalCount) = await _targetRepository.ListAsync(dto.Year, dto.TargetType, dto.Page, dto.PageSize);
        return new ListingDto<TargetDto>
        {
            Data = _mapper.Map<List<TargetDto>>(targets),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
