using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.ContractApprovals;

public class ListContractApprovalsRequest : BasePagedRequest
{
    public const string Permission = "view_contract";

    public long? ContractId { get; set; }
    public ApprovalStage? Stage { get; set; }
    public long? ApproverId { get; set; }
    public ApprovalStatus? Status { get; set; }

    public ListContractApprovalsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        ContractId = ContractId,
        Stage = Stage,
        ApproverId = ApproverId,
        Status = Status
    };
}
