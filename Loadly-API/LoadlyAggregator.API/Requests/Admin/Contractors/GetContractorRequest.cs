using LoadlyAggregator.Domain.Admin.DTOs.Contractors;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class GetContractorRequest
{
    public const string Permission = "view_contractor";

    public long Id { get; set; }

    public GetContractorDto FromRequest() => new() { Id = Id };
}
