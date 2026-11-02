using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Common.Services;

/// <summary>
/// Applies bank-transfer allocations to an invoice and syncs its status.
/// Transaction-agnostic: the caller must already have an open transaction and is
/// responsible for persisting the mutated invoice (SaveChanges/Commit). Shared by
/// the create invoice flow so validation + status-sync logic lives once.
/// </summary>
public class InvoicePaymentAllocator
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IInvoicePaymentRepository _invoicePaymentRepository;

    public InvoicePaymentAllocator(
        IBankTransferRepository bankTransferRepository,
        IInvoicePaymentRepository invoicePaymentRepository)
    {
        _bankTransferRepository = bankTransferRepository;
        _invoicePaymentRepository = invoicePaymentRepository;
    }

    /// <summary>
    /// Upserts one (invoice, bankTransfer) allocation. Delegates to <see cref="ApplyBatchAsync"/>.
    /// </summary>
    public Task ApplyAsync(Invoice invoice, long bankTransferId, decimal amount, long? addedBy) =>
        ApplyBatchAsync(invoice, [(bankTransferId, amount)], addedBy);

    /// <summary>
    /// Batch upsert with in-tx running totals: one bank-transfer load, one allocated-total
    /// query, one lock acquisition, no per-row SaveChanges required by the caller.
    /// Requires <paramref name="invoice"/>.Contract to be loaded for contract billing.
    /// </summary>
    public async Task ApplyBatchAsync(
        Invoice invoice,
        IReadOnlyList<(long BankTransferId, decimal Amount)> allocations,
        long? addedBy)
    {
        if (allocations.Count == 0)
            return;

        var bankTransferIds = allocations.Select(a => a.BankTransferId).Distinct().ToList();
        var bankTransferById = (await _bankTransferRepository.GetByIdsForAllocationOrThrowAsync(bankTransferIds))
            .ToDictionary(b => b.Id);

        await _invoicePaymentRepository.AcquireBatchAllocationLocksAsync(invoice.Id, bankTransferIds);

        var transferAllocated = await _invoicePaymentRepository.GetTotalAllocatedByBankTransferIdsAsync(bankTransferIds);
        var existingByTransferId = ActivePayments(invoice).ToDictionary(p => p.BankTransferId);
        var invoicePaid = SumActivePayments(invoice);

        foreach (var (bankTransferId, amount) in allocations)
        {
            var bankTransfer = bankTransferById[bankTransferId];
            var existing = existingByTransferId.GetValueOrDefault(bankTransferId);
            var previousForPair = existing?.Amount ?? 0m;

            BankTransferAllocationValidator.ValidateAllocation(
                invoice,
                bankTransfer,
                amount,
                invoicePaid,
                transferAllocated.GetValueOrDefault(bankTransferId),
                existing);

            if (existing is not null)
            {
                existing.Amount = amount;
                if (addedBy.HasValue)
                    existing.AddedBy = addedBy;
                _invoicePaymentRepository.Update(existing);
            }
            else
            {
                var payment = new InvoicePayment
                {
                    InvoiceId = invoice.Id,
                    BankTransferId = bankTransferId,
                    Amount = amount,
                    AddedBy = addedBy
                };
                await _invoicePaymentRepository.AddAsync(payment);
                existingByTransferId[bankTransferId] = payment;
                invoice.Payments.Add(payment);
            }

            invoicePaid = invoicePaid - previousForPair + amount;
            transferAllocated[bankTransferId] = transferAllocated.GetValueOrDefault(bankTransferId) - previousForPair + amount;
        }

        BankTransferAllocationValidator.SyncInvoiceStatus(invoice, invoicePaid);
    }

    /// <summary>
    /// Unlinks invoice payments in memory, then re-syncs status from the recomputed paid total.
    /// Bank transfers are left untouched. Caller flushes once after any follow-up applies.
    /// </summary>
    public Task RemoveRangeAsync(Invoice invoice, IReadOnlyCollection<InvoicePayment> invoicePaymentsToUnlink)
    {
        if (invoicePaymentsToUnlink.Count == 0)
            return Task.CompletedTask;

        var invoicePaid = SumActivePayments(invoice);
        var unlinkedAmount = invoicePaymentsToUnlink.Sum(p => p.Amount);

        _invoicePaymentRepository.RemoveRange(invoicePaymentsToUnlink);
        BankTransferAllocationValidator.SyncInvoiceStatus(invoice, invoicePaid - unlinkedAmount);
        return Task.CompletedTask;
    }

    private static IEnumerable<InvoicePayment> ActivePayments(Invoice invoice) =>
        invoice.Payments.Where(p => p.DeletedAt == null);

    private static decimal SumActivePayments(Invoice invoice) =>
        ActivePayments(invoice).Sum(p => p.Amount);
}
