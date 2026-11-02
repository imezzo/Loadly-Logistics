using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Roles;

public class GetRoleAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetRoleAction(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<RoleDto> ExecuteAsync(GetRoleDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);

        if (role.GuardName != "admin" || role.CompanyId != null)
        {
            throw new NotFoundException("roles.not_found");
        }

        return _mapper.Map<RoleDto>(role);
    }
}
