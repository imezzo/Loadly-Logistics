using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class BankTransfer : BaseEntity
{
    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public string? Attachment { get; set; }
    public long? PayerId { get; set; }
    public string? PayerType { get; set; }
    public long? AddedBy { get; set; }
    public virtual Company? Company { get; set; }
    public virtual Lead? Lead { get; set; }
    public virtual ICollection<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
}
