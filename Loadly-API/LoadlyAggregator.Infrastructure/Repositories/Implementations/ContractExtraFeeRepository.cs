using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractExtraFeeRepository : Repository<ContractExtraFee>, IContractExtraFeeRepository
{
    public ContractExtraFeeRepository(ApplicationDbContext context) : base(context)
    {
    }
    public override Task<ContractExtraFee?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cef => cef.Contract)
            .FirstOrDefaultAsync(cef => cef.Id == id);

    public Task<List<ContractExtraFee>> GetByContractIdAsync(long contractId) =>
        _dbSet
            .Include(cef => cef.Contract)
            .Where(cef => cef.ContractId == contractId)
            .ToListAsync();

    public Task<List<ContractExtraFee>> GetByContractIdsAsync(IEnumerable<long> contractIds) =>
        _dbSet
            .Include(cef => cef.Contract)
            .Where(cef => contractIds.Contains(cef.ContractId))
            .ToListAsync();

    public Task<ContractExtraFee?> GetByContractIdAndExtraFeeTypeIdAsync(long contractId, int extraFeeTypeId) =>
        _dbSet
            .Include(cef => cef.Contract)
            .FirstOrDefaultAsync(cef => cef.ContractId == contractId && cef.ExtraFeeTypeId == extraFeeTypeId);

    public Task<List<ContractExtraFee>> GetByExtraFeeTypeIdAsync(int extraFeeTypeId) =>
        _dbSet
            .Include(cef => cef.Contract)
            .Where(cef => cef.ExtraFeeTypeId == extraFeeTypeId)
            .ToListAsync();

    public override IQueryable<ContractExtraFee> Query() =>
        _dbSet.Include(cef => cef.Contract).AsQueryable();

    public async Task<(List<ContractExtraFee> Items, int TotalCount)> ListAsync(
        long? contractId,
        int? extraFeeTypeId,
        int page,
        int pageSize) =>
        (await _dbSet
            .Include(cef => cef.Contract)
            .Where(cef => !contractId.HasValue || cef.ContractId == contractId.Value)
            .Where(cef => !extraFeeTypeId.HasValue || cef.ExtraFeeTypeId == extraFeeTypeId.Value)
            .OrderByDescending(cef => cef.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet
            .Where(cef => !contractId.HasValue || cef.ContractId == contractId.Value)
            .Where(cef => !extraFeeTypeId.HasValue || cef.ExtraFeeTypeId == extraFeeTypeId.Value)
            .CountAsync());

    public async Task<ContractExtraFee> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contract_extra_fees.not_found_with_id", id);
}
