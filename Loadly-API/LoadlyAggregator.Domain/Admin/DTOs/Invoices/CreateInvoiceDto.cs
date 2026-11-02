using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class CreateInvoiceDto
{
    public InvoiceType InvoiceType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<long> TripIds { get; set; } = [];
    public long? ContractBillingSnapshotId { get; set; }
    public bool ExcludeExtraKmsFromMainInvoice { get; set; }
    public long ContractId { get; set; }
    public long? LeadId { get; set; }
    public long? TaxId { get; set; }
    public long? DiscountId { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly IssueDate { get; set; }
    public FileUpload? TaxedInvoiceAttachment { get; set; }
    public InvoiceStatus? Status { get; set; }
    public List<InvoicePaymentAllocationDto>? Payments { get; set; }
}
