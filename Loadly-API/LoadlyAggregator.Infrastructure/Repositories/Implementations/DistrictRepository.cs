using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DistrictRepository : Repository<District>, IDistrictRepository
{
    public DistrictRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<District?> GetByIdAsync(long id) =>
        await _dbSet.Include(d => d.City).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<District?> GetByNameAndCityAsync(string nameEn, long cityId) =>
        await _dbSet
            .FirstOrDefaultAsync(d =>
                d.NameEn.ToLower() == nameEn.ToLower() &&
                d.CityId == cityId);

    public async Task<List<District>> GetByCityAndNamesAsync(long cityId, IReadOnlyCollection<string> districtNames) =>
        await _dbSet.Where(d => d.CityId == cityId && districtNames.Contains(d.NameEn.ToLower())).ToListAsync();

    public async Task<List<District>> GetByCityIdsAndDistrictNamesAsync(IReadOnlyCollection<long> cityIds, IReadOnlyCollection<string> districtNames) =>
        await _dbSet.Where(d => cityIds.Contains(d.CityId) && districtNames.Contains(d.NameEn.ToLower())).ToListAsync();

    public override async Task AddAsync(District district) =>
        await base.AddAsync(district);

    public override void Remove(District district) =>
        base.Remove(district);

    public override IQueryable<District> Query() =>
        base.Query();

    public async Task<(List<District> Districts, int TotalCount)> ListAsync(
        string? search,
        long? cityId,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Include(d => d.City)
            .Where(d => string.IsNullOrWhiteSpace(search) ||
                d.NameEn.Contains(search!) ||
                (d.NameAr != null && d.NameAr.Contains(search!)))
            .Where(d => !cityId.HasValue || d.CityId == cityId.Value)
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(d => string.IsNullOrWhiteSpace(search) ||
                d.NameEn.Contains(search!) ||
                (d.NameAr != null && d.NameAr.Contains(search!)))
            .Where(d => !cityId.HasValue || d.CityId == cityId.Value)
            .CountAsync());

    public async Task<District> GetByIdOrThrowAsync(long id) =>
        await _dbSet.Include(d => d.City).FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new NotFoundException("districts.not_found_with_id", id);
}

