using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IHasRoles
{
    long Id { get; set; }
    string GuardName { get; }
    ICollection<ModelHasPermission> DirectPermissions { get; set; }
    ICollection<ModelHasRole> UserRoles { get; set; }
}
