using System.Linq;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IGovernerateRepository : IRepository<Common.Entities.Governerate>
{
    Task<Common.Entities.Governerate?> GetByNameAsync(string nameEn);
    Task<IReadOnlyDictionary<string, long>> GetIdMapByNamesAsync(IReadOnlyCollection<string> names);

    Task<(List<Common.Entities.Governerate> Governerates, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
    Task<Common.Entities.Governerate> GetByIdOrThrowAsync(long id);
}
