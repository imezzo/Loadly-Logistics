using System.Linq;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Shared approval defaults for contract workflow (DRY). Uses pipeline for stages (OCP).
/// </summary>
public static class ContractApprovalHelper
{
    /// <summary>
    /// Creates default approval records from the pipeline stages for a new or resubmitted contract.
    /// </summary>
    public static List<ContractApproval> CreateDefaultApprovals(long contractId, IReadOnlyList<ContractApprovalStageInfo> stages)
    {
        return stages
            .Select(s => new ContractApproval { ContractId = contractId, Stage = s.Stage, Status = ApprovalStatus.PENDING, StageOrder = s.StageOrder })
            .ToList();
    }
}
