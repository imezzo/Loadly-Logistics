using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

[NoAudit]
public class ContractStatusHistory : BaseEntity
{
    public long ContractId { get; set; }
    public ContractStatus PreviousStatus { get; set; }
    public ContractStatus NewStatus { get; set; }
    /// <summary>Approval stage this history entry relates to (when status change is approval-related); null otherwise.</summary>
    public ApprovalStage? ApprovalStage { get; set; }
    public long? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? AdditionalNotes { get; set; }

    public virtual Contract? Contract { get; set; }
    public virtual Admin? ChangedByAdmin { get; set; }
}
