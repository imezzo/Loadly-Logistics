using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class ListCompanyUsersRequest : BasePagedRequest
{
    public const string Permission = "view_company";
    [FromRoute]
    public long CompanyId { get; set; }
    public int? Status { get; set; }

    public ListCompanyUsersDto FromRequest() => new()
    {
        CompanyId = CompanyId,
        Search = Search,
        Status = Status,
        Page = Page,
        PageSize = PageSize
    };
}
