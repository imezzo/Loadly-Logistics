
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
namespace LoadlyAggregator.API.Requests.Admin.Roles;

public class DeleteRoleRequest
{
    public const string Permission = "manage_permissions";

    public long Id { get; set; }

    public DeleteRoleDto FromRequest() => new() { Id = Id };
}
