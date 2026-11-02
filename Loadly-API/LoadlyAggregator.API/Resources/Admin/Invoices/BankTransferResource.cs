using LoadlyAggregator.API.Resources.RelationResources;

namespace LoadlyAggregator.API.Resources.Admin.Invoices;

public class BankTransferResource
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public int PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public string? Attachment { get; set; }
    public string? FileUrl { get; set; }
    public long? PayerId { get; set; }
    public string? PayerType { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public LeadRelationResource? Lead { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int InvoicePaymentCount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string DisplayLabelEn { get; set; } = string.Empty;
    public string DisplayLabelAr { get; set; } = string.Empty;
    public List<InvoicePaymentRelationResource> InvoicePayments { get; set; } = new();
}
