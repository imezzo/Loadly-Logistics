using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class VehicleDetailRepository
    : Repository<VehicleDetail>, IVehicleDetailRepository
{
    public VehicleDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<VehicleDetail?> GetByVehicleIdAsync(long vehicleId)
    {
        return await _dbSet
            .Include(vd => vd.Vehicle) // remove if you don’t need navigation
            .FirstOrDefaultAsync(vd => vd.VehicleId == vehicleId);
    }
}
