using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Services;

/// <summary>
/// Provides the ordered approval stages and their permission keys (OCP: extend by new implementation or config, no switch in callers).
/// </summary>
public interface IContractApprovalPipeline
{
    /// <summary>Ordered list of stages for default approval creation.</summary>
    IReadOnlyList<ContractApprovalStageInfo> GetStages();

    /// <summary>Permission key required to approve/reject at the given stage.</summary>
    string GetRequiredPermission(ApprovalStage stage);
}

/// <summary>Single stage definition in the pipeline.</summary>
public record ContractApprovalStageInfo(ApprovalStage Stage, int StageOrder, string RequiredPermissionKey);
