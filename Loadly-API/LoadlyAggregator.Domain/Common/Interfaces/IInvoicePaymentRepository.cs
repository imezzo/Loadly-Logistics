using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IInvoicePaymentRepository : IRepository<InvoicePayment>
{
    Task AcquireAllocationLocksAsync(long invoiceId, long bankTransferId);

    Task AcquireBatchAllocationLocksAsync(long invoiceId, IReadOnlyList<long> bankTransferIds);

    Task<Dictionary<long, decimal>> GetTotalAllocatedByBankTransferIdsAsync(IReadOnlyList<long> bankTransferIds);
    Task<List<InvoicePayment>> GetByInvoiceIdAsync(long invoiceId);
    Task<InvoicePayment?> GetByInvoiceAndBankTransferAsync(long invoiceId, long bankTransferId);
    Task<InvoicePayment> UpsertAmountAsync(long invoiceId, long bankTransferId, decimal amount, long? addedBy);
    Task<decimal> GetTotalPaidAmountByInvoiceIdAsync(long invoiceId);
    Task<decimal> GetTotalAllocatedAmountByBankTransferIdAsync(long bankTransferId);
    Task<InvoicePayment> GetByIdOrThrowAsync(long id);
    Task<(List<InvoicePayment> Items, int TotalCount)> ListAsync(
        long? invoiceId,
        int page,
        int pageSize);
}
