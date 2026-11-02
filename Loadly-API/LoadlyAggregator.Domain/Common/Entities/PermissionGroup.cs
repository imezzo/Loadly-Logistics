using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class PermissionGroup : BaseEntity
{
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public PermissionStatus Status { get; set; } = PermissionStatus.ENABLED;
    public string Type { get; set; } = "company";

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
