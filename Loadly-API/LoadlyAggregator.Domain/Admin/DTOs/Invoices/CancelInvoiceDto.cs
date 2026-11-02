namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class CancelInvoiceDto
{
    public long Id { get; set; }
    public string? CancellationReason { get; set; }
}
