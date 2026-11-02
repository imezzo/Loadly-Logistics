
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class DeleteCompanyRequest
{
    public const string Permission = "delete_company";

    public long Id { get; set; }

    public DeleteCompanyDto FromRequest() => new() { Id = Id };
}
