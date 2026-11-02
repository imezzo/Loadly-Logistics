using LoadlyAggregator.Domain.Admin.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Admin.RoleHasPermissions;

public class GetRolePermissionsRequest
{
    public const string Permission = "manage_permissions";

    public long RoleId { get; set; }

    public GetRolePermissionsDto FromRequest() => new() { RoleId = RoleId };
}
