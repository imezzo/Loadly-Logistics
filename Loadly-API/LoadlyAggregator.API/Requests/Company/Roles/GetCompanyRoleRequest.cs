using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Company.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Company.Roles;

public class GetCompanyRoleRequest
{
    public const string Permission = "manage_permissions";

    [FromRoute]
    public long Id { get; set; }

    public GetCompanyRoleDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId
    };
}
