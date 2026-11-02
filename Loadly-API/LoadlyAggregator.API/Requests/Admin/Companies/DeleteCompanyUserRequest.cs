using LoadlyAggregator.Domain.Admin.DTOs.Companies;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class DeleteCompanyUserRequest
{
    public const string Permission = "delete_company";
    public long CompanyId { get; set; }
    public long Id { get; set; }
    public DeleteCompanyUserDto FromRequest() => new DeleteCompanyUserDto
    {
        Id = Id,
        CompanyId = CompanyId
    };
}
