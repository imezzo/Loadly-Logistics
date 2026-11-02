using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class IndustryTypeRepository : Repository<IndustryType>, IIndustryTypeRepository
{
    public IndustryTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IndustryType?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<IndustryType?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(i => i.NameEn.ToLower() == nameEn.ToLower());

    public async Task<IndustryType> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("industry_types.not_found_with_id", id);

    public async Task<(List<IndustryType> IndustryTypes, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(i => string.IsNullOrWhiteSpace(search) ||
                i.NameEn.Contains(search!) ||
                (i.NameAr != null && i.NameAr.Contains(search!)))
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(i => string.IsNullOrWhiteSpace(search) ||
                i.NameEn.Contains(search!) ||
                (i.NameAr != null && i.NameAr.Contains(search!)))
            .CountAsync());

    public override async Task AddAsync(IndustryType industryType) =>
        await base.AddAsync(industryType);

    public override void Remove(IndustryType industryType) =>
        base.Remove(industryType);

    public override IQueryable<IndustryType> Query() =>
        base.Query();
}

