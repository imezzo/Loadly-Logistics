using System.Linq;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDistrictRepository : IRepository<Common.Entities.District>
{
    Task<Common.Entities.District?> GetByNameAndCityAsync(string nameEn, long cityId);
    Task<List<Common.Entities.District>> GetByCityAndNamesAsync(long cityId, IReadOnlyCollection<string> districtNames);
    Task<List<Common.Entities.District>> GetByCityIdsAndDistrictNamesAsync(IReadOnlyCollection<long> cityIds, IReadOnlyCollection<string> districtNames);

    Task<(List<Common.Entities.District> Districts, int TotalCount)> ListAsync(
        string? search,
        long? cityId,
        int page,
        int pageSize);
    Task<Common.Entities.District> GetByIdOrThrowAsync(long id);
}
