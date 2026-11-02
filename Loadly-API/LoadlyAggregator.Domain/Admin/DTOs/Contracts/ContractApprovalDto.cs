using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ContractApprovalDto
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public ApprovalStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public long? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string? ApproverRole { get; set; }
    public ApprovalStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
    public int StageOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContractStatusHistoryDto
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public ContractStatus PreviousStatus { get; set; }
    public string PreviousStatusName { get; set; } = string.Empty;
    public ContractStatus NewStatus { get; set; }
    public string NewStatusName { get; set; } = string.Empty;
    public ApprovalStage? ApprovalStage { get; set; }
    public string? ApprovalStageName { get; set; }
    public long? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? AdditionalNotes { get; set; }
}

public class ContractApprovalDecisionDto
{
    public long ContractId { get; set; }
    public ContractApprovalDecision Action { get; set; }
    public string? Comments { get; set; }
}

public class GetApprovalStatusDto
{
    public long ContractId { get; set; }
}

public class ListContractApprovalsDto : BasePagedRequest
{
    public long? ContractId { get; set; }
    public ApprovalStage? Stage { get; set; }
    public long? ApproverId { get; set; }
    public ApprovalStatus? Status { get; set; }
}
