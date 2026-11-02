using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class GetCollectorAction
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IMapper _mapper;

    public GetCollectorAction(ICollectorRepository collectorRepository, IMapper mapper)
    {
        _collectorRepository = collectorRepository;
        _mapper = mapper;
    }

    public async Task<CollectorDto> ExecuteAsync(GetCollectorDto dto)
    {
        var collector = await _collectorRepository.GetByIdOrThrowAsync(dto.Id);
        var roles = collector.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList();
        var collectorDto = _mapper.Map<CollectorDto>(collector);
        collectorDto.Roles = _mapper.Map<List<RoleDto>>(roles);
        return collectorDto;
    }
}
