namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class InvoicePaymentAllocationDto
{
    public long BankTransferId { get; set; }
    public decimal Amount { get; set; }
}
