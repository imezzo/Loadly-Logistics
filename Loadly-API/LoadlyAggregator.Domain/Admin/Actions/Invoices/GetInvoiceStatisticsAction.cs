using System.Linq;
using LoadlyAggregator.Domain.Common.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class GetInvoiceStatisticsAction
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceStatisticsAction(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceStatisticsDto> ExecuteAsync()
    {
        var invoices = await _invoiceRepository.GetAllForStatisticsAsync();

        var paidInvoicesCount = 0;
        var unpaidInvoicesCount = 0;
        var partiallyPaidInvoicesCount = 0;
        decimal totalInvoicesAmount = 0m;
        decimal totalPaidAmount = 0m;

        foreach (var invoice in invoices)
        {
            var paidForInvoice = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
            totalInvoicesAmount += invoice.TotalAmount;
            totalPaidAmount += paidForInvoice;

            switch (invoice.Status)
            {
                case InvoiceStatus.Paid:
                    paidInvoicesCount++;
                    break;
                case InvoiceStatus.UnPaid:
                    unpaidInvoicesCount++;
                    break;
                case InvoiceStatus.PartiallyPaid:
                    partiallyPaidInvoicesCount++;
                    break;
            }
        }

        return new InvoiceStatisticsDto
        {
            PaidInvoicesCount = paidInvoicesCount,
            UnpaidInvoicesCount = unpaidInvoicesCount,
            PartiallyPaidInvoicesCount = partiallyPaidInvoicesCount,
            TotalInvoicesAmount = totalInvoicesAmount,
            TotalPaidAmount = totalPaidAmount,
            TotalRemainingAmount = totalInvoicesAmount - totalPaidAmount
        };
    }
}
