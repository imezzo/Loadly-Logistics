using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Roles;

public class DeleteRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public DeleteRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(DeleteRoleDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);
        if (role.GuardName != "admin" || role.CompanyId != null)
        {
            throw new NotFoundException("roles.not_found");
        }

        if (await _roleRepository.HasAssignedUsersAsync(dto.Id))
        {
            throw new LogicalException("roles.cannot_delete_assigned");
        }
        await _roleRepository.DeleteAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}

