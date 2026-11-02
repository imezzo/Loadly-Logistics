namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IColorRepository : IRepository<Common.Entities.Color>
{
    Task<Common.Entities.Color?> GetByColorHashAsync(string colorHash);

    Task<(List<Common.Entities.Color> Colors, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
    Task<Common.Entities.Color> GetByIdOrThrowAsync(long id);
    Task<Dictionary<string, long>> GetIdMapByNamesAsync(List<string> namesLower);
}
