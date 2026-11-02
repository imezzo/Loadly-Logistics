using LoadlyAggregator.Domain.Admin.DTOs.Contractors;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class DeleteContractorRequest
{
    public const string Permission = "delete_contractor";

    public long Id { get; set; }

    public DeleteContractorDto FromRequest() => new() { Id = Id };
}
