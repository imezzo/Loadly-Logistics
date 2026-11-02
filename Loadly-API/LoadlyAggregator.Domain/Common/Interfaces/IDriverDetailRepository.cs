using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDriverDetailRepository : IRepository<Common.Entities.DriverDetail>
{
    Task<Common.Entities.DriverDetail?> GetByDriverIdAsync(long driverId);
    Task<Common.Entities.DriverDetail?> GetByLicenseNoAsync(string licenseNo);
    Task<bool> ExistsByLicenseNoAsync(string licenseNo, long? excludeId = null);
    Task<IReadOnlyDictionary<long, Common.Entities.DriverDetail>> GetMapByDriverIdsAsync(IReadOnlyCollection<long> driverIds);
}
