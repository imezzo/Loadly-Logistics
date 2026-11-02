using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DriverLicenseTypeRepository : Repository<DriverLicenseType>, IDriverLicenseTypeRepository
{
    public DriverLicenseTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<DriverLicenseType?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<DriverLicenseType?> GetByNameAsync(string nameEn) =>
        await _dbSet.FirstOrDefaultAsync(dlt => dlt.NameEn == nameEn);

    public async Task<DriverLicenseType> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("driver_license_types.not_found_with_id", id);

    public async Task<(List<DriverLicenseType> DriverLicenseTypes, int TotalCount)> ListAsync(
        string? search,
        UserStatus? status,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(dlt => string.IsNullOrWhiteSpace(search) ||
                dlt.NameEn.Contains(search!) ||
                dlt.NameAr.Contains(search!))
            .Where(dlt => !status.HasValue || dlt.Status == status.Value)
            .OrderByDescending(dlt => dlt.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(dlt => string.IsNullOrWhiteSpace(search) ||
                dlt.NameEn.Contains(search!) ||
                dlt.NameAr.Contains(search!))
            .Where(dlt => !status.HasValue || dlt.Status == status.Value)
            .CountAsync());
}

