using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class ListCollectorsAction
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ListCollectorsAction(
        ICollectorRepository collectorRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _collectorRepository = collectorRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ListingDto<CollectorDto>> ExecuteAsync(ListCollectorsDto dto)
    {
        var (collectors, totalCount) = await _collectorRepository.ListAsync(
            dto.Search,
            dto.Status,
            _currentUserService.GetCurrentUserId(),
            dto.Page,
            dto.PageSize);

        var collectorDtos = collectors.Select(collector =>
        {
            var roles = collector.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList();
            var collectorDto = _mapper.Map<CollectorDto>(collector);
            collectorDto.Roles = _mapper.Map<List<RoleDto>>(roles);
            return collectorDto;
        }).ToList();

        return new ListingDto<CollectorDto>
        {
            Data = collectorDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
