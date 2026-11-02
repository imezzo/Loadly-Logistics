using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class BankTransferRepository : Repository<BankTransfer>, IBankTransferRepository
{
    public BankTransferRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BankTransfer> GetByIdOrThrowAsync(long id) =>
        await QueryWithPayerIncludes()
            .FirstOrDefaultAsync(b => b.Id == id)
        ?? throw new NotFoundException("bank_transfers.not_found_with_id", id);

    public async Task<BankTransfer> GetForAllocationOrThrowAsync(long id) =>
        (await GetByIdsForAllocationOrThrowAsync([id])).First();

    public async Task<IReadOnlyList<BankTransfer>> GetByIdsForAllocationOrThrowAsync(IReadOnlyList<long> ids)
    {
        if (ids.Count == 0)
            return [];

        var distinctIds = ids.Distinct().ToList();
        var items = await _dbSet
            .AsNoTracking()
            .Where(b => distinctIds.Contains(b.Id))
            .Select(b => new BankTransfer
            {
                Id = b.Id,
                Amount = b.Amount,
                PayerId = b.PayerId,
                PayerType = b.PayerType
            })
            .ToListAsync();

        if (items.Count != distinctIds.Count)
        {
            var found = items.Select(b => b.Id).ToHashSet();
            var missing = distinctIds.First(id => !found.Contains(id));
            throw new NotFoundException("bank_transfers.not_found_with_id", missing);
        }

        return items;
    }

    public async Task<(List<BankTransfer> Items, int TotalCount)> ListAsync(
        PaymentMethod? paymentMethod,
        string? payerType,
        IReadOnlyList<long>? payerIds,
        string? search,
        int page,
        int pageSize)
    {
        var query = _dbSet.AsQueryable()
            .Where(b => !paymentMethod.HasValue || b.PaymentMethod == paymentMethod.Value)
            .Where(b => string.IsNullOrWhiteSpace(payerType) || b.PayerType == payerType)
            .Where(b => payerIds == null || payerIds.Count == 0 ||
                (b.PayerId.HasValue && payerIds.Contains(b.PayerId.Value)))
            .Where(b => string.IsNullOrWhiteSpace(search) ||
                (b.ExternalPaymentReferenceNumber != null && b.ExternalPaymentReferenceNumber.Contains(search!)))
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await QueryWithPayerIncludes(query)
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    private IQueryable<BankTransfer> QueryWithPayerIncludes(IQueryable<BankTransfer>? query = null) =>
        (query ?? _dbSet)
            .Include(b => b.Company!)
                .ThenInclude(c => c!.CompanyDetail)
            .Include(b => b.Lead)
            .Include(b => b.InvoicePayments)
                .ThenInclude(p => p.Invoice);
}
