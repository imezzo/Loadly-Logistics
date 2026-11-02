using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Roles;

public class DeleteCompanyRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public DeleteCompanyRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(DeleteCompanyRoleDto dto)
    {
        if (await _roleRepository.HasAssignedUsersAsync(dto.Id))
        {
            throw new LogicalException("roles.cannot_delete_assigned");
        }

        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);

        if (role.GuardName != "company" || role.CompanyId != dto.CompanyId)
        {
            throw new NotFoundException("roles.not_found");
        }

        await _roleRepository.DeleteAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}
