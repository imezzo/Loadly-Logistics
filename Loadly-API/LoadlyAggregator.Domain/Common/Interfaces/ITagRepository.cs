namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITagRepository : IRepository<Common.Entities.Tag>
{
    Task<Common.Entities.Tag?> GetByNameEnAsync(string nameEn);
    Task<(List<Common.Entities.Tag> Tags, int TotalCount)> ListAsync(
        string? search,
        bool? isPricing,
        int page,
        int pageSize);
    Task<Common.Entities.Tag> GetByIdOrThrowAsync(long id);
}
