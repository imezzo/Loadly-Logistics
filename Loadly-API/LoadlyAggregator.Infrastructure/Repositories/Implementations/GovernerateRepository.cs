using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class GovernerateRepository : Repository<Governerate>, IGovernerateRepository
{
    public GovernerateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Governerate?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Governerate?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(g => g.NameEn.ToLower() == nameEn.ToLower());

    public async Task<IReadOnlyDictionary<string, long>> GetIdMapByNamesAsync(IReadOnlyCollection<string> names) =>
      (await _dbSet.Where(g => names.Contains(g.NameEn.ToLower())).Select(g => new { g.NameEn, g.Id }).ToListAsync())
      .ToDictionary(x => x.NameEn.ToLowerInvariant(), x => x.Id);

    public override async Task AddAsync(Governerate governerate) =>
        await base.AddAsync(governerate);

    public override void Remove(Governerate governerate) =>
        base.Remove(governerate);

    public override IQueryable<Governerate> Query() =>
        base.Query();

    public async Task<(List<Governerate> Governerates, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(g => string.IsNullOrWhiteSpace(search) ||
                g.NameEn.Contains(search!) ||
                (g.NameAr != null && g.NameAr.Contains(search!)))
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(g => string.IsNullOrWhiteSpace(search) ||
                g.NameEn.Contains(search!) ||
                (g.NameAr != null && g.NameAr.Contains(search!)))
            .CountAsync());

    public async Task<Governerate> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("governerates.not_found_with_id", id);
}

