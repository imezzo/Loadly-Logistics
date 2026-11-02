namespace LoadlyAggregator.Domain.Company.DTOs.RoleHasPermissions;

public class SyncCompanyRoleHasPermissionDto
{
    public long RoleId { get; set; }
    public long CompanyId { get; set; }
    public List<long> PermissionIds { get; set; } = new();
}

public class GetCompanyRolePermissionsDto
{
    public long RoleId { get; set; }
    public long CompanyId { get; set; }
}
