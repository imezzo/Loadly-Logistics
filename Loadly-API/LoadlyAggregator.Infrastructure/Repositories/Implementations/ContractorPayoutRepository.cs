using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractorPayoutRepository : Repository<ContractorPayout>, IContractorPayoutRepository
{
    public ContractorPayoutRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<ContractorPayout?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(p => p.Contractor)
            .Include(p => p.Trips)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<ContractorPayout> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contractor_payouts.not_found_with_id", id);

    public async Task<(List<ContractorPayout> Payouts, int TotalCount)> ListAsync(
        long? contractorId,
        ContractorPayoutStatus? status,
        int page,
        int pageSize)
    {
        var filtered = _dbSet
            .AsNoTracking()
            .Where(p => !contractorId.HasValue || p.ContractorId == contractorId.Value)
            .Where(p => !status.HasValue || p.Status == status.Value);

        var totalCount = await filtered.CountAsync();
        var payouts = await filtered
            .Include(p => p.Contractor)
            .Include(p => p.Trips)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (payouts, totalCount);
    }

    private const int PayoutSequenceLockNamespace = 847292;

    public async Task<int> AllocateNextPayoutSequenceForYearAsync(int year)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock({0}::integer, {1}::integer)",
            PayoutSequenceLockNamespace,
            year);

        var maxSequence = await GetMaxPayoutSequenceForYearAsync(year);
        return (maxSequence ?? 0) + 1;
    }

    public async Task<int?> GetMaxPayoutSequenceForYearAsync(int year)
    {
        var prefix = $"CPY-{year}-";
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(p => p.PayoutNumber.StartsWith(prefix) && p.Sequence != null)
            .Select(p => p.Sequence)
            .MaxAsync();
    }
}
