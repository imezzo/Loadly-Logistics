using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DriverDetailRepository : Repository<DriverDetail>, IDriverDetailRepository
{
    public DriverDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<DriverDetail?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<DriverDetail?> GetByDriverIdAsync(long driverId)
    {
        return await _dbSet.FirstOrDefaultAsync(dd => dd.DriverId == driverId);
    }
    public async Task<DriverDetail?> GetByLicenseNoAsync(string licenseNo) =>
    await _context.Set<DriverDetail>().FirstOrDefaultAsync(d => d.LicenseNo == licenseNo);
    public async Task<bool> ExistsByLicenseNoAsync(string licenseNo, long? excludeId = null) =>
       await _context.Set<DriverDetail>()
           .Where(d => d.LicenseNo == licenseNo)
           .Where(d => !excludeId.HasValue || d.DriverId != excludeId.Value)
           .AnyAsync();

    public async Task<IReadOnlyDictionary<long, DriverDetail>> GetMapByDriverIdsAsync(IReadOnlyCollection<long> driverIds)
    {
        if (driverIds.Count == 0)
            return new Dictionary<long, DriverDetail>();

        List<DriverDetail> rows = await _dbSet
            .AsNoTracking()
            .Where(dd => driverIds.Contains(dd.DriverId))
            .ToListAsync();

        return rows.ToDictionary(dd => dd.DriverId);
    }

    public override async Task AddAsync(DriverDetail driverDetail)
    {
        await base.AddAsync(driverDetail);
    }

    public override void Remove(DriverDetail driverDetail)
    {
        base.Remove(driverDetail);
    }
}

