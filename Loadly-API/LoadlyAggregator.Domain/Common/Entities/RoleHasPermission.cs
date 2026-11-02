namespace LoadlyAggregator.Domain.Common.Entities;

public class RoleHasPermission
{
    public long PermissionId { get; set; }
    public long RoleId { get; set; }

    public virtual Permission Permission { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
