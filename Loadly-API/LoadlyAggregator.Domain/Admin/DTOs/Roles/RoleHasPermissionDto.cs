namespace LoadlyAggregator.Domain.Admin.DTOs.Roles;

public class SyncRoleHasPermissionDto
{
    public long RoleId { get; set; }
    public List<long> PermissionIds { get; set; } = new();
}

public class GetRolePermissionsDto
{
    public long RoleId { get; set; }
}
