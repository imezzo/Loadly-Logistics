using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class ListCompaniesRequest : BasePagedRequest
{
    public const string Permission = "view_company";

    public int? Status { get; set; }

    public ListCompaniesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}
