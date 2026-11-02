using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Company.DTOs.Users;

namespace LoadlyAggregator.API.Requests.Company.Users;

public class GetCompanyUserRequest
{
    public const string Permission = "view_user";

    [FromRoute]
    public long Id { get; set; }

    public CompanyUserManagementDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId
    };
}
