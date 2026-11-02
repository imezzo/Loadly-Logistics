
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
namespace LoadlyAggregator.API.Resources.Admin.Roles;

/// <summary>
/// Resource for Role API responses (both listing and single get)
/// </summary>
public class RoleResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PermissionRelationResource> Permissions { get; set; } = new();

    public static RoleResource FromDto(RoleDto dto) =>
        new RoleResource
        {
            Id = dto.Id,
            Name = dto.Name,
            NameAr = dto.NameAr,
            GuardName = dto.GuardName,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Permissions = dto.Permissions.Select(p => new PermissionRelationResource
            {
                Id = p.Id,
                Name = p.Name,
                NameAr = p.NameAr,
                GuardName = p.GuardName
            }).ToList()
        };
}

public class PermissionRelationResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
}
