using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class InvoiceDetail : BaseEntity
{
    public long InvoiceId { get; set; }
    public long? AddedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public long? PaidBy { get; set; }
    public DateTime? PaidAt { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Admin? AddedByAdmin { get; set; }
    public virtual Admin? UpdatedByAdmin { get; set; }
    public virtual Admin? PaidByAdmin { get; set; }
}
