namespace LoadlyAggregator.Domain.Common.DTOs.Invoices;

public class InvoiceStatisticsDto
{
    public int PaidInvoicesCount { get; set; }
    public int UnpaidInvoicesCount { get; set; }
    public int PartiallyPaidInvoicesCount { get; set; }
    public decimal TotalInvoicesAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRemainingAmount { get; set; }
}
