using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
    public PermissionStatus Status { get; set; } = PermissionStatus.ENABLED;
    public long PermissionGroupId { get; set; }

    public virtual PermissionGroup PermissionGroup { get; set; } = null!;
    public virtual ICollection<RoleHasPermission> RolePermissions { get; set; } = new List<RoleHasPermission>();
    public virtual ICollection<ModelHasPermission> ModelPermissions { get; set; } = new List<ModelHasPermission>();
}
