using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractApproval : BaseEntity
{
    public long ContractId { get; set; }
    public ApprovalStage Stage { get; set; }
    public long? ApproverId { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.PENDING;
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
    public int StageOrder { get; set; }
    public byte[] RowVersion { get; set; } = null!;

    public virtual Contract? Contract { get; set; }
    public virtual Admin? Approver { get; set; }
}
