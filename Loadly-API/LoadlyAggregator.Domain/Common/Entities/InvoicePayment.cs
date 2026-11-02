using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class InvoicePayment : BaseEntity
{
    public long InvoiceId { get; set; }
    public long BankTransferId { get; set; }
    public decimal Amount { get; set; }
    public long? AddedBy { get; set; }
    public virtual Invoice? Invoice { get; set; }
    public virtual BankTransfer BankTransfer { get; set; } = null!;
}
