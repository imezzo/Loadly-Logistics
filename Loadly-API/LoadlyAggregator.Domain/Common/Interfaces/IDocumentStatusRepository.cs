namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDocumentStatusRepository : IRepository<Common.Entities.LookupDocumentStatus>
{
    Task<Common.Entities.LookupDocumentStatus?> GetByNameEnAsync(string nameEn);
    Task<Common.Entities.LookupDocumentStatus?> GetByNameArAsync(string nameAr);

    Task<(List<Common.Entities.LookupDocumentStatus> DocumentStatuses, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Common.Entities.LookupDocumentStatus> GetByIdOrThrowAsync(long id);
}
