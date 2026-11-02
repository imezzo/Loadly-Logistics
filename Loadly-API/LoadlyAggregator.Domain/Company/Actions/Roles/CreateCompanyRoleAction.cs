using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Roles;

public class CreateCompanyRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public CreateCompanyRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(CreateCompanyRoleDto dto)
    {
        var existing = await _roleRepository.GetByNameAndGuardAndCompanyAsync(dto.Name, "company", dto.CompanyId);
        if (existing != null)
        {
            throw new ValidationException("roles.name_guard_already_exists", dto.Name, "company");
        }

        var role = new Role
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            GuardName = "company",
            Description = dto.Description,
            CompanyId = dto.CompanyId,
        };

        await _roleRepository.AddAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}
