using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ColorRepository : Repository<Color>, IColorRepository
{
    public ColorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Color?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<Color?> GetByColorHashAsync(string colorHash) =>
        await _dbSet
            .FirstOrDefaultAsync(c => c.ColorHash.ToLower() == colorHash.ToLower());

    public override async Task AddAsync(Color color) =>
        await base.AddAsync(color);

    public override void Remove(Color color) =>
        base.Remove(color);

    public override IQueryable<Color> Query() =>
        base.Query();

    public async Task<(List<Color> Colors, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)) ||
                c.ColorHash.Contains(search!))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)) ||
                c.ColorHash.Contains(search!))
            .CountAsync());

    public async Task<Color> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("colors.not_found_with_id", id);

    public async Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower)
    {
        var colors = await _dbSet
            .Where(c => namesLower.Contains(c.NameEn.ToLower()))
            .ToListAsync();
        return colors
            .GroupBy(c => c.NameEn.ToLower())
            .ToDictionary(g => g.Key, g => g.First().Id);
    }
}

