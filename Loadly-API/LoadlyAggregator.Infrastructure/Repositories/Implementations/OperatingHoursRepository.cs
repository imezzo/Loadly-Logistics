using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class OperatingHoursRepository : Repository<OperatingHour>, IOperatingHoursRepository
{
    public OperatingHoursRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<OperatingHour?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<OperatingHour?> GetByNameAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(c => c.NameEn.ToLower() == nameEn.ToLower());

    public async Task<OperatingHour> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("operating_hours.not_found_with_id", id);

    public async Task<(List<OperatingHour> OperatingHours, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.NameEn.Contains(search!) ||
                (c.NameAr != null && c.NameAr.Contains(search!)))
            .CountAsync());

    public override async Task AddAsync(OperatingHour operatingHour) =>
        await base.AddAsync(operatingHour);

    public override void Remove(OperatingHour operatingHour) =>
        base.Remove(operatingHour);

    public override IQueryable<OperatingHour> Query() =>
        base.Query();
}
