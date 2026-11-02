using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Roles;

public class CreateRoleAction
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleAction(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(CreateRoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            GuardName = "admin",
            Description = dto.Description,
        };
        await _roleRepository.AddAsync(role);
        await _roleRepository.SaveChangesAsync();
    }
}
