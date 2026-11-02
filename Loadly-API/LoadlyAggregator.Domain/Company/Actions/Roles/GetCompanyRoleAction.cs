using AutoMapper;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Roles;

public class GetCompanyRoleAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetCompanyRoleAction(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<CompanyRoleDto> ExecuteAsync(GetCompanyRoleDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);

        if (role.GuardName != "company" || role.CompanyId != dto.CompanyId)
        {
            throw new NotFoundException("roles.not_found");
        }

        return _mapper.Map<CompanyRoleDto>(role);
    }
}
