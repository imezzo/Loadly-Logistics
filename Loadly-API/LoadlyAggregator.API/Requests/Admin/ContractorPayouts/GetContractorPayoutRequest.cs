using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

namespace LoadlyAggregator.API.Requests.Admin.ContractorPayouts;

public class GetContractorPayoutRequest
{
    public const string Permission = "view_contractor_payout";

    public long Id { get; set; }

    public GetContractorPayoutDto FromRequest() => new()
    {
        Id = Id
    };
}
