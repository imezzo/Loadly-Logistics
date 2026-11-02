using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Roles;

public class UpdateRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public UpdateRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);
        if (role.GuardName != "admin" || role.CompanyId != null)
        {
            throw new NotFoundException("roles.not_found");
        }

        role.Name = dto.Name;
        role.NameAr = dto.NameAr;
        role.GuardName = "admin";
        role.Description = dto.Description;
        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}
