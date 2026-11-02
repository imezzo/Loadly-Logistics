using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyDictionary<long, City>> GetMapByIdsAsync(IReadOnlyCollection<long> cityIds)
    {
        if (cityIds.Count == 0)
            return new Dictionary<long, City>();

        List<City> cities = await _dbSet
            .AsNoTracking()
            .Where(c => cityIds.Contains(c.Id))
            .ToListAsync();

        return cities.ToDictionary(c => c.Id);
    }

    public override async Task<City?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(c => c.Governerate)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<City?> GetByNameAndGovernerateAsync(string nameEn, long governerateId) =>
        await _dbSet
            .FirstOrDefaultAsync(c =>
                c.NameEn.ToLower() == nameEn.ToLower() &&
                c.GovernerateId == governerateId);

    public async Task<List<City>> GetByGovernorateAndNamesAsync(long governerateId, IReadOnlyCollection<string> cityNames) =>
        await _dbSet.Where(c => c.GovernerateId == governerateId && cityNames.Contains(c.NameEn.ToLower())).ToListAsync();

    public async Task<List<City>> GetByGovernorateIdsAndCityNamesAsync(IReadOnlyCollection<long> governerateIds, IReadOnlyCollection<string> cityNames) =>
        await _dbSet.Where(c => governerateIds.Contains(c.GovernerateId) && cityNames.Contains(c.NameEn.ToLower())).ToListAsync();

    public override async Task AddAsync(City city) =>
        await base.AddAsync(city);

    public override void Remove(City city) =>
        base.Remove(city);

    public override IQueryable<City> Query() =>
        base.Query();

    public async Task<(List<City> Cities, int TotalCount)> ListAsync(
        string? search,
        long? governerateId,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
                .AsNoTracking()
                .Include(c => c.Governerate)
                .Where(c => string.IsNullOrWhiteSpace(search) ||
                    c.NameEn.Contains(search!) ||
                    (c.NameAr != null && c.NameAr.Contains(search!)))
                .Where(c => !governerateId.HasValue || c.GovernerateId == governerateId.Value)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            await _dbSet.AsQueryable()
                .Where(c => string.IsNullOrWhiteSpace(search) ||
                    c.NameEn.Contains(search!) ||
                    (c.NameAr != null && c.NameAr.Contains(search!)))
                .Where(c => !governerateId.HasValue || c.GovernerateId == governerateId.Value)
                .CountAsync());

    public async Task<City> GetByIdOrThrowAsync(long id) =>
        await _dbSet
            .Include(c => c.Governerate)
            .FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new NotFoundException("cities.not_found_with_id", id);
}

