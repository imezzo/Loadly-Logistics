using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleBrandModelRepository : Repository<VehicleBrandModel>, IVehicleBrandModelRepository
{
    public VehicleBrandModelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleBrandModel?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(v => v.VehicleBrand)
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task<VehicleBrandModel?> GetByNameEnAndBrandIdAsync(string nameEn, long vehicleBrandId) =>
        await _dbSet
            .FirstOrDefaultAsync(v =>
                v.NameEn.ToLower() == nameEn.ToLower() &&
                v.VehicleBrandId == vehicleBrandId);

    public async Task<(List<VehicleBrandModel> VehicleBrandModels, int TotalCount)> ListAsync(
        string? search,
        long? vehicleBrandId,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.NameEn.Contains(search!) ||
                (v.NameAr != null && v.NameAr.Contains(search!)))
            .Where(v => !vehicleBrandId.HasValue || v.VehicleBrandId == vehicleBrandId.Value)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.NameEn.Contains(search!) ||
                (v.NameAr != null && v.NameAr.Contains(search!)))
            .Where(v => !vehicleBrandId.HasValue || v.VehicleBrandId == vehicleBrandId.Value)
            .CountAsync());

    public async Task<VehicleBrandModel> GetByIdOrThrowAsync(long id) =>
        await _dbSet
            .Include(v => v.VehicleBrand)
            .FirstOrDefaultAsync(v => v.Id == id) ?? throw new NotFoundException("vehicle_brand_models.not_found_with_id", id);

    public async Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower)
    {
        var models = await _dbSet
            .Where(m => namesLower.Contains(m.NameEn.ToLower()))
            .ToListAsync();
        return models
            .GroupBy(m => m.NameEn.ToLower())
            .ToDictionary(g => g.Key, g => g.First().Id);
    }
}

