using LoadlyAggregator.Domain.Company.DTOs.Users;

namespace LoadlyAggregator.API.Requests.Company.Users;

public class ListCompanyUsersRequest
{
    public const string Permission = "view_user";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? Status { get; set; }

    public CompanyUserManagementDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}
