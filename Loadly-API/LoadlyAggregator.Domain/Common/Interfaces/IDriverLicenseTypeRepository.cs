using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDriverLicenseTypeRepository : IRepository<Common.Entities.DriverLicenseType>
{
    Task<Common.Entities.DriverLicenseType?> GetByNameAsync(string nameEn);

    Task<(List<Common.Entities.DriverLicenseType> DriverLicenseTypes, int TotalCount)> ListAsync(
        string? search,
        UserStatus? status,
        int page,
        int pageSize);

    Task<Common.Entities.DriverLicenseType> GetByIdOrThrowAsync(long id);
}
