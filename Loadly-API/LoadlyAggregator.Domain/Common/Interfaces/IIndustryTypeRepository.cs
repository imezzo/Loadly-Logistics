namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IIndustryTypeRepository : IRepository<Common.Entities.IndustryType>
{
    Task<Common.Entities.IndustryType?> GetByNameAsync(string nameEn);
    Task<Common.Entities.IndustryType> GetByIdOrThrowAsync(long id);
    Task<(List<Common.Entities.IndustryType> IndustryTypes, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
}
