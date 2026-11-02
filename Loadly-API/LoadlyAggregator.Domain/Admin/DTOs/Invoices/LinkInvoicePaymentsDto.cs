namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class LinkInvoicePaymentsDto
{
    public long Id { get; set; }
    public List<LinkInvoicePaymentItemDto> Payments { get; set; } = new();
}
