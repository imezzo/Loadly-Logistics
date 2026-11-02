namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ISectorRepository : IRepository<Common.Entities.Sector>
{
    Task<Common.Entities.Sector?> GetByNameAsync(string nameEn);

    Task<(List<Common.Entities.Sector> Sectors, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Common.Entities.Sector> GetByIdOrThrowAsync(long id);
}
