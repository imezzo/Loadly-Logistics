using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Roles;

public class ListRolesAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public ListRolesAction(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<RoleDto>> ExecuteAsync(ListRolesDto dto)
    {
        var (roles, totalCount) = await _roleRepository.ListAsync(dto.Search, "admin", dto.Page, dto.PageSize);
        return new ListingDto<RoleDto>
        {
            Data = _mapper.Map<List<RoleDto>>(roles),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
