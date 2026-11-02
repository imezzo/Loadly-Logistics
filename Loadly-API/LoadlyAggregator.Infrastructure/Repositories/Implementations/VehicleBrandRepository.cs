using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleBrandRepository : Repository<VehicleBrand>, IVehicleBrandRepository
{
    public VehicleBrandRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<VehicleBrand?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<VehicleBrand?> GetByNameEnAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(v => v.NameEn.ToLower() == nameEn.ToLower());

    public async Task<(List<VehicleBrand> VehicleBrands, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.NameEn.Contains(search!) ||
                (v.NameAr != null && v.NameAr.Contains(search!)))
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(v => string.IsNullOrWhiteSpace(search) ||
                v.NameEn.Contains(search!) ||
                (v.NameAr != null && v.NameAr.Contains(search!)))
            .CountAsync());

    public async Task<VehicleBrand> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("vehicle_brands.not_found_with_id", id);
}

