namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class LinkInvoicePaymentItemDto
{
    public long? InvoicePaymentId { get; set; }
    public long BankTransferId { get; set; }
    public decimal Amount { get; set; }
}
