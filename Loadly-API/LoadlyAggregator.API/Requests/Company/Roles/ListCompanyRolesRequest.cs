using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Company.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Company.Roles;

public class ListCompanyRolesRequest : BasePagedRequest
{
    public const string Permission = "manage_permissions";

    public ListCompanyRolesDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}
