using System.Collections.Generic;
using System.Linq;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICityRepository : IRepository<Common.Entities.City>
{
    Task<IReadOnlyDictionary<long, Common.Entities.City>> GetMapByIdsAsync(IReadOnlyCollection<long> cityIds);

    Task<Common.Entities.City?> GetByNameAndGovernerateAsync(string nameEn, long governerateId);
    Task<List<Common.Entities.City>> GetByGovernorateAndNamesAsync(long governerateId, IReadOnlyCollection<string> cityNames);
    Task<List<Common.Entities.City>> GetByGovernorateIdsAndCityNamesAsync(IReadOnlyCollection<long> governerateIds, IReadOnlyCollection<string> cityNames);

    Task<(List<Common.Entities.City> Cities, int TotalCount)> ListAsync(
        string? search,
        long? governerateId,
        int page,
        int pageSize);
    Task<Common.Entities.City> GetByIdOrThrowAsync(long id);
}
