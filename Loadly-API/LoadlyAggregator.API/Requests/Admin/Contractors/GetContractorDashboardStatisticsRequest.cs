using LoadlyAggregator.Domain.Admin.DTOs.Contractors;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class GetContractorDashboardStatisticsRequest
{
    public const string Permission = "view_contractor";

    public long? ContractorId { get; set; }

    public GetContractorDashboardStatisticsDto FromRequest() => new()
    {
        ContractorId = ContractorId
    };
}
