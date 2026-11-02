using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class SectorRepository : Repository<Sector>, ISectorRepository
{
    public SectorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Sector?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Sector?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(s => s.NameEn.ToLower() == nameEn.ToLower());

    public override async Task AddAsync(Sector sector) =>
        await base.AddAsync(sector);

    public override void Remove(Sector sector) =>
        base.Remove(sector);

    public override IQueryable<Sector> Query() =>
        base.Query();

    public async Task<(List<Sector> Sectors, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(s => string.IsNullOrWhiteSpace(search) ||
                s.NameEn.Contains(search!) ||
                (s.NameAr != null && s.NameAr.Contains(search!)))
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(s => string.IsNullOrWhiteSpace(search) ||
                s.NameEn.Contains(search!) ||
                (s.NameAr != null && s.NameAr.Contains(search!)))
            .CountAsync());

    public async Task<Sector> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("sectors.not_found_with_id", id);
}
