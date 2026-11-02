using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class GetCompanyUserRequest
{
    public const string Permission = "view_company";
    [FromRoute]
    public long CompanyId { get; set; }
    [FromRoute]
    public long Id { get; set; }
    public GetCompanyUserDto FromRequest() => new GetCompanyUserDto
    {
        CompanyId = CompanyId,
        Id = Id
    };
}
