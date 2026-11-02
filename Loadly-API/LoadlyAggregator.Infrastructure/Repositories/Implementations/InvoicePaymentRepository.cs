using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class InvoicePaymentRepository : Repository<InvoicePayment>, IInvoicePaymentRepository
{
    private const int InvoiceAllocationLockNamespace = 847292;
    private const int BankTransferAllocationLockNamespace = 847293;

    public InvoicePaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task AcquireAllocationLocksAsync(long invoiceId, long bankTransferId) =>
        AcquireBatchAllocationLocksAsync(invoiceId, [bankTransferId]);

    public async Task AcquireBatchAllocationLocksAsync(long invoiceId, IReadOnlyList<long> bankTransferIds)
    {
        var locks = new List<(long Namespace, long Id)>
        {
            (InvoiceAllocationLockNamespace, invoiceId)
        };
        locks.AddRange(bankTransferIds.Distinct().Select(id => ((long)BankTransferAllocationLockNamespace, id)));
        locks.Sort((a, b) =>
        {
            var nsCompare = a.Namespace.CompareTo(b.Namespace);
            return nsCompare != 0 ? nsCompare : a.Id.CompareTo(b.Id);
        });

        foreach (var (ns, id) in locks)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_xact_lock({0}::integer, {1}::integer)",
                ns,
                id);
        }
    }

    public override async Task<InvoicePayment?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(p => p.Invoice)
            .Include(p => p.BankTransfer)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<InvoicePayment> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("invoice_payments.not_found_with_id", id);

    public async Task<List<InvoicePayment>> GetByInvoiceIdAsync(long invoiceId) =>
        await _dbSet
            .Where(p => p.InvoiceId == invoiceId)
            .ToListAsync();

    public async Task<InvoicePayment?> GetByInvoiceAndBankTransferAsync(long invoiceId, long bankTransferId) =>
        await _dbSet.FirstOrDefaultAsync(p =>
            p.InvoiceId == invoiceId && p.BankTransferId == bankTransferId);

    public async Task<InvoicePayment> UpsertAmountAsync(
        long invoiceId,
        long bankTransferId,
        decimal amount,
        long? addedBy)
    {
        var existing = await GetByInvoiceAndBankTransferAsync(invoiceId, bankTransferId);
        if (existing is not null)
        {
            existing.Amount = amount;
            if (addedBy.HasValue)
                existing.AddedBy = addedBy;

            Update(existing);
            return existing;
        }

        var payment = new InvoicePayment
        {
            InvoiceId = invoiceId,
            BankTransferId = bankTransferId,
            Amount = amount,
            AddedBy = addedBy
        };
        await AddAsync(payment);
        return payment;
    }

    public async Task<decimal> GetTotalPaidAmountByInvoiceIdAsync(long invoiceId) =>
        await _dbSet
            .Where(p => p.InvoiceId == invoiceId)
            .SumAsync(p => p.Amount);

    public async Task<decimal> GetTotalAllocatedAmountByBankTransferIdAsync(long bankTransferId) =>
        (await GetTotalAllocatedByBankTransferIdsAsync([bankTransferId])).GetValueOrDefault(bankTransferId);

    public async Task<Dictionary<long, decimal>> GetTotalAllocatedByBankTransferIdsAsync(IReadOnlyList<long> bankTransferIds)
    {
        if (bankTransferIds.Count == 0)
            return new Dictionary<long, decimal>();

        var distinctIds = bankTransferIds.Distinct().ToList();
        var sums = await _dbSet
            .Where(p => distinctIds.Contains(p.BankTransferId))
            .GroupBy(p => p.BankTransferId)
            .Select(g => new { BankTransferId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync();

        return distinctIds.ToDictionary(
            id => id,
            id => sums.FirstOrDefault(s => s.BankTransferId == id)?.Total ?? 0m);
    }

    public async Task<(List<InvoicePayment> Items, int TotalCount)> ListAsync(
        long? invoiceId,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsQueryable()
            .Include(p => p.Invoice)
            .Include(p => p.BankTransfer)
            .Where(p => !invoiceId.HasValue || p.InvoiceId == invoiceId.Value)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
