
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
namespace LoadlyAggregator.API.Requests.Admin.Roles;

public class GetRoleRequest
{
    public const string Permission = "manage_permissions";

    public long Id { get; set; }

    public GetRoleDto FromRequest() => new() { Id = Id };
}
