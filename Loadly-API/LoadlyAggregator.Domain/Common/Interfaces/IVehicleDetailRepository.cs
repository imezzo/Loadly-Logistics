using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleDetailRepository : IRepository<VehicleDetail>
{
    Task<VehicleDetail?> GetByVehicleIdAsync(long vehicleId);
}
