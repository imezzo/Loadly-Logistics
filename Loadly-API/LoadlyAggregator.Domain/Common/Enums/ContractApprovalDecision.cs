using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum ContractApprovalDecision
{
    [Display("Approve", "موافقة")]
    Approve = 1,
    [Display("Reject", "رفض")]
    Reject = 2
}
