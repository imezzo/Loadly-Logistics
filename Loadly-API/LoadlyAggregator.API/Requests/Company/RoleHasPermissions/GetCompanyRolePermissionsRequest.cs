using LoadlyAggregator.Domain.Company.DTOs.RoleHasPermissions;

namespace LoadlyAggregator.API.Requests.Company.RoleHasPermissions;

public class GetCompanyRolePermissionsRequest
{
    public const string Permission = "manage_permissions";

    public GetCompanyRolePermissionsDto FromRequest(long roleId, long companyId) => new()
    {
        RoleId = roleId,
        CompanyId = companyId
    };
}
