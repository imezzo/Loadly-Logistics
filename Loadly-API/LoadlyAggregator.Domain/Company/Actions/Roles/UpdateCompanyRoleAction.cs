using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Roles;

public class UpdateCompanyRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public UpdateCompanyRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(UpdateCompanyRoleDto dto)
    {
        var role = await _roleRepository.GetByIdOrThrowAsync(dto.Id);

        if (role.GuardName != "company" || role.CompanyId != dto.CompanyId)
        {
            throw new NotFoundException("roles.not_found");
        }

        var sameNameRole = await _roleRepository.GetByNameAndGuardAndCompanyAsync(dto.Name, "company", dto.CompanyId);
        if (sameNameRole != null && sameNameRole.Id != dto.Id)
        {
            throw new ValidationException("roles.name_guard_already_exists", dto.Name, "company");
        }

        role.Name = dto.Name;
        role.NameAr = dto.NameAr;
        role.Description = dto.Description;
        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}
