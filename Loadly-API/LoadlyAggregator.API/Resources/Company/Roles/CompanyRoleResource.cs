using LoadlyAggregator.Domain.Company.DTOs.Roles;

namespace LoadlyAggregator.API.Resources.Company.Roles;

public class CompanyRoleResource
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CompanyRolePermissionResource> Permissions { get; set; } = new();

    public static CompanyRoleResource FromDto(CompanyRoleDto dto) => new()
    {
        Id = dto.Id,
        CompanyId = dto.CompanyId,
        Name = dto.Name,
        NameAr = dto.NameAr,
        Description = dto.Description,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        Permissions = dto.Permissions.Select(p => new CompanyRolePermissionResource
        {
            Id = p.Id,
            Name = p.Name,
            NameAr = p.NameAr,
            GuardName = p.GuardName
        }).ToList()
    };
}

public class CompanyRolePermissionResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
}
