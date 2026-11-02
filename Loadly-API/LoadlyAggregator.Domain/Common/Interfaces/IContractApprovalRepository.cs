using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractApprovalRepository : IRepository<ContractApproval>
{
    Task<List<ContractApproval>> GetApprovalsByContractIdAsync(long contractId);
    Task<ContractApproval?> GetApprovalByContractAndStageAsync(long contractId, ApprovalStage stage);
    Task<ContractApproval?> GetCurrentApprovalAsync(long contractId);
    /// <summary>Next pending approval with StageOrder greater than afterStageOrder.</summary>
    Task<ContractApproval?> GetNextPendingApprovalAsync(long contractId, int afterStageOrder);
    /// <summary>True if the user has an APPROVED approval for this contract at a different stage (excluding excludeApprovalId).</summary>
    Task<bool> HasUserAlreadyApprovedAtOtherStageAsync(long contractId, long userId, long excludeApprovalId);
    /// <summary>Pending approvals for the contract (status PENDING).</summary>
    Task<List<ContractApproval>> GetPendingApprovalsByContractIdAsync(long contractId);
    /// <summary>Pending approvals for the contract excluding the one with excludeApprovalId.</summary>
    Task<List<ContractApproval>> GetOtherPendingApprovalsAsync(long contractId, long excludeApprovalId);
    /// <summary>Latest rejected approval for the contract (by ActionDate), or null if none.</summary>
    Task<ContractApproval?> GetLatestRejectionByContractIdAsync(long contractId);
    Task<List<ContractApproval>> GetPendingApprovalsByApproverIdAsync(long approverId);
    /// <summary>True if the contract has any approval rows (any status).</summary>
    Task<bool> HasApprovalsForContractAsync(long contractId);
    Task<bool> HasPendingApprovalsAsync(long contractId);
    Task<(List<ContractApproval> ContractApprovals, int TotalCount)> ListAsync(
        long? contractId,
        ApprovalStage? stage,
        long? approverId,
        ApprovalStatus? status,
        int page,
        int pageSize);
    Task<ContractApproval> GetByIdOrThrowAsync(long id);
}
