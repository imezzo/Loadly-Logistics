using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.ContractApprovals;

public class GetApprovalStatusRequest
{
    public const string Permission = "view_contract";

    public long ContractId { get; set; }

    public GetApprovalStatusDto FromRequest() => new()
    {
        ContractId = ContractId
    };
}
