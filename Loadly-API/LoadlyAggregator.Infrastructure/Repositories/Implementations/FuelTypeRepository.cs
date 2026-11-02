using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class FuelTypeRepository : Repository<FuelType>, IFuelTypeRepository
{
    public FuelTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<FuelType?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<FuelType?> GetByNameEnAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(f => f.NameEn.ToLower() == nameEn.ToLower());

    public async Task<FuelType?> GetByNameArAsync(string nameAr) =>
        await _dbSet
            .FirstOrDefaultAsync(f => f.NameAr.ToLower() == nameAr.ToLower());

    public override async Task AddAsync(FuelType fuelType) =>
        await base.AddAsync(fuelType);

    public override void Remove(FuelType fuelType) =>
        base.Remove(fuelType);

    public override IQueryable<FuelType> Query() =>
        base.Query();

    public async Task<(List<FuelType> FuelTypes, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(f => string.IsNullOrWhiteSpace(search) ||
                f.NameEn.Contains(search!) ||
                (f.NameAr != null && f.NameAr.Contains(search!)))
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(f => string.IsNullOrWhiteSpace(search) ||
                f.NameEn.Contains(search!) ||
                (f.NameAr != null && f.NameAr.Contains(search!)))
            .CountAsync());

    public async Task<FuelType> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("fuel_types.not_found_with_id", id);
}

