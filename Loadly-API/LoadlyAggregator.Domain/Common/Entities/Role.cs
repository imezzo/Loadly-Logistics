using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? CompanyId { get; set; }

    public virtual Company? Company { get; set; }
    public virtual ICollection<ModelHasRole> UserRoles { get; set; } = new List<ModelHasRole>();
    public virtual ICollection<RoleHasPermission> RolePermissions { get; set; } = new List<RoleHasPermission>();
}
