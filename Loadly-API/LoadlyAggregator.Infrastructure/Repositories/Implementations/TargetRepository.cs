using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TargetRepository : Repository<Target>, ITargetRepository
{
    public TargetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Target?> GetByMonthYearAndTypeAsync(
        TargetMonth month,
        int year,
        TargetType targetType,
        long? excludeId = null) =>
        await Query()
            .Where(t =>
                t.Month == month
                && t.Year == year
                && t.TargetType == targetType
                && (!excludeId.HasValue || t.Id != excludeId.Value))
            .FirstOrDefaultAsync();

    public async Task<(List<Target> Targets, int TotalCount)> ListAsync(int? year, TargetType? targetType, int page, int pageSize) =>
        (await Query()
            .AsNoTracking()
            .Where(t => !year.HasValue || t.Year == year.Value)
            .Where(t => !targetType.HasValue || t.TargetType == targetType.Value)
            .OrderByDescending(t => t.Year)
            .ThenBy(t => t.Month)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await Query()
            .Where(t => !year.HasValue || t.Year == year.Value)
            .Where(t => !targetType.HasValue || t.TargetType == targetType.Value)
            .CountAsync());

    public async Task<Target> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("targets.not_found_with_id", id);

    public async Task<IReadOnlyList<Target>> GetMonthlyTargetsAsync(int? year = null, int? month = null) =>
        await Query()
            .Where(t => !year.HasValue || t.Year == year.Value)
            .Where(t => !month.HasValue || (int)t.Month == month.Value)
            .OrderBy(t => t.Year)
            .ThenBy(t => t.Month)
            .ToListAsync();
}
