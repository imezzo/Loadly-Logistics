using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice> GetByIdOrThrowAsync(long id);
    Task<List<Invoice>> GetAllForStatisticsAsync();
    Task<List<Invoice>> GetAllForStatisticsForCompanyAsync(long companyId);
    Task<(List<Invoice> Invoices, int TotalCount)> ListAsync(
        string? search,
        long? companyId,
        long? contractId,
        InvoiceStatus? status,
        InvoiceType? invoiceType,
        DateOnly? startDateFrom,
        DateOnly? startDateTo,
        bool? isOverdue,
        bool? isLeadBilling,
        int page,
        int pageSize);
    Task<int?> GetMaxInvoiceSequenceForYearAsync(int year);
    Task<int> GetCompanyPaidInvoicesCountInRangeAsync(long companyId, DateTime fromInclusive, DateTime toExclusive);
    Task<int> AllocateNextInvoiceSequenceForYearAsync(int year);

    Task<bool> HasOverlappingTripsInvoiceForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        long? excludeInvoiceId = null);

    Task<bool> HasOverlappingInvoiceForContractAsync(
        long contractId,
        DateOnly periodStart,
        DateOnly periodEnd,
        InvoiceType invoiceType,
        long? excludeInvoiceId = null);
}