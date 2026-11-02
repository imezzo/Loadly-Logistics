
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class GetCompanyRequest
{
    public const string Permission = "view_company";

    public long Id { get; set; }

    public GetCompanyDto FromRequest() => new() { Id = Id };
}
