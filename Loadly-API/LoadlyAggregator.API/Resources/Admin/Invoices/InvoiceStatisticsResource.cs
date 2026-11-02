using LoadlyAggregator.Domain.Common.DTOs.Invoices;

namespace LoadlyAggregator.API.Resources.Admin.Invoices;

public class InvoiceStatisticsResource
{
    public int PaidInvoicesCount { get; set; }
    public int UnpaidInvoicesCount { get; set; }
    public int PartiallyPaidInvoicesCount { get; set; }
    public decimal TotalInvoicesAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRemainingAmount { get; set; }

    public static InvoiceStatisticsResource FromDto(InvoiceStatisticsDto dto) =>
        new()
        {
            PaidInvoicesCount = dto.PaidInvoicesCount,
            UnpaidInvoicesCount = dto.UnpaidInvoicesCount,
            PartiallyPaidInvoicesCount = dto.PartiallyPaidInvoicesCount,
            TotalInvoicesAmount = dto.TotalInvoicesAmount,
            TotalPaidAmount = dto.TotalPaidAmount,
            TotalRemainingAmount = dto.TotalRemainingAmount
        };
}
