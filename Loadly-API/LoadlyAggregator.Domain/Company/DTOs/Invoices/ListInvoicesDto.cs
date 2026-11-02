using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Company.DTOs.Invoices;

public class ListInvoicesDto : BasePagedRequest
{
    public long CompanyId { get; set; }
    public long? ContractId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsLeadBilling { get; set; }
}
