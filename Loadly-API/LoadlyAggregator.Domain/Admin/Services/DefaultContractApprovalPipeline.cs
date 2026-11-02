using System.Linq;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Services;

/// <summary>
/// Default three-stage contract approval pipeline. Replace or extend via DI to add stages without changing action code.
/// </summary>
public sealed class DefaultContractApprovalPipeline : IContractApprovalPipeline
{
    private static readonly IReadOnlyList<ContractApprovalStageInfo> Stages = new[]
    {
        new ContractApprovalStageInfo(ApprovalStage.SALES_MANAGER, 1, "approve_contract_sales"),
        new ContractApprovalStageInfo(ApprovalStage.OPERATIONS_MANAGER, 2, "approve_contract_operations"),
        new ContractApprovalStageInfo(ApprovalStage.FINANCE_MANAGER, 3, "approve_contract_finance")
    };

    public IReadOnlyList<ContractApprovalStageInfo> GetStages() => Stages;

    public string GetRequiredPermission(ApprovalStage stage)
    {
        var info = Stages.FirstOrDefault(s => s.Stage == stage);
        return info?.RequiredPermissionKey ?? string.Empty;
    }
}
