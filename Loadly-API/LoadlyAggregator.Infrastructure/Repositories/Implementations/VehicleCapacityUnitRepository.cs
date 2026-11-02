using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleCapacityUnitRepository : Repository<VehicleCapacityUnit>, IVehicleCapacityUnitRepository
{
    public VehicleCapacityUnitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleCapacityUnit?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<VehicleCapacityUnit?> GetByCapacityTypeEnAsync(string capacityTypeEn) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.CapacityTypeEn.ToLower() == capacityTypeEn.ToLower());

    public async Task<VehicleCapacityUnit> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("vehicle_capacity_units.not_found_with_id", id);

    public async Task<(List<VehicleCapacityUnit> VehicleCapacityUnits, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.CapacityTypeEn.Contains(search!) ||
                (v.CapacityTypeAr != null && v.CapacityTypeAr.Contains(search!)))
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.CapacityTypeEn.Contains(search!) ||
                (v.CapacityTypeAr != null && v.CapacityTypeAr.Contains(search!)))
            .CountAsync());

    public async Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower)
    {
        var units = await _dbSet
            .Where(u => namesLower.Contains(u.CapacityTypeEn.ToLower()))
            .ToListAsync();
        return units
            .GroupBy(u => u.CapacityTypeEn.ToLower())
            .ToDictionary(g => g.Key, g => g.First().Id);
    }
}

