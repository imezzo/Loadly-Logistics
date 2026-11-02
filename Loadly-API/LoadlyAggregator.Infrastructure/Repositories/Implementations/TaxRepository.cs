using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TaxRepository : Repository<Tax>, ITaxRepository
{
    public TaxRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Tax?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Tax?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(t => t.NameEn.ToLower() == nameEn.ToLower());

    public override async Task AddAsync(Tax tax) =>
        await base.AddAsync(tax);

    public override void Remove(Tax tax) =>
        base.Remove(tax);

    public override IQueryable<Tax> Query() =>
        base.Query();

    public async Task<(List<Tax> Taxes, int TotalCount)> ListAsync(
        string? search,
        TaxType? type,
        TaxStatus? status,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(t => string.IsNullOrWhiteSpace(search) ||
                t.NameEn.Contains(search!) ||
                t.NameAr.Contains(search!))
            .Where(t => !type.HasValue || t.Type == type.Value)
            .Where(t => !status.HasValue || t.Status == status.Value)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(t => string.IsNullOrWhiteSpace(search) ||
                t.NameEn.Contains(search!) ||
                t.NameAr.Contains(search!))
            .Where(t => !type.HasValue || t.Type == type.Value)
            .Where(t => !status.HasValue || t.Status == status.Value)
            .CountAsync());

    public async Task<Tax> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("taxes.not_found_with_id", id);
}

