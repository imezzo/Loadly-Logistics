using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Roles;

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = new();
}

public class RolePermissionDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string GuardName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetRoleDto
{
    public long Id { get; set; }
}

public class ListRolesDto : BasePagedRequest
{
    public string? GuardName { get; set; }
}

public class UpdateRoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string GuardName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class DeleteRoleDto
{
    public long Id { get; set; }
}
