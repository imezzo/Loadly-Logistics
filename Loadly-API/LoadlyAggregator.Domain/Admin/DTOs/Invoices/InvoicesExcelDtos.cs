using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public sealed class InvoicesExcelFileDto
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
}

public sealed class ExportInvoicesExcelDto
{
    public string? Search { get; init; }
    public long? CompanyId { get; init; }
    public long? ContractId { get; init; }
    public InvoiceStatus? Status { get; init; }
    public InvoiceType? InvoiceType { get; init; }
    public DateOnly? StartDateFrom { get; init; }
    public DateOnly? StartDateTo { get; init; }
    public bool? IsOverdue { get; init; }
    public bool? IsLeadBilling { get; init; }
    public List<string>? Columns { get; init; }
}
