using System.Linq;
using LoadlyAggregator.Domain.Common.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public class CompanyGetInvoiceStatisticsAction(IInvoiceRepository invoiceRepository)
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;

    public async Task<InvoiceStatisticsDto> ExecuteAsync(CompanyGetInvoiceStatisticsDto dto)
    {
        var invoices = await _invoiceRepository.GetAllForStatisticsForCompanyAsync(dto.CompanyId);

        int paidInvoicesCount = 0;
        int unpaidInvoicesCount = 0;
        int partiallyPaidInvoicesCount = 0;
        decimal totalInvoicesAmount = 0m;
        decimal totalPaidAmount = 0m;

        foreach (var invoice in invoices)
        {
            decimal paidForInvoice = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
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
