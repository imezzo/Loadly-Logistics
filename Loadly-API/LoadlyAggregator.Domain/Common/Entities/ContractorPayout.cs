using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractorPayout : BaseEntity
{
    public string PayoutNumber { get; set; } = string.Empty;
    public int? Sequence { get; set; }
    public long ContractorId { get; set; }
    public decimal TotalAmount { get; set; }
    public ContractorPayoutStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public long? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public virtual Contractor? Contractor { get; set; }
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
