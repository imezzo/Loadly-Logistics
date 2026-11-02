using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByEntityIdAsync(DocumentEntityType entityId);

    Task<IEnumerable<Document>> GetRequiredActiveByEntityAsync(DocumentEntityType entityId);

    Task<(IEnumerable<Document> Documents, int TotalCount)> SearchAsync(
        string? search = null,
        DocumentEntityType? entityId = null,
        bool? required = null,
        DocumentStatus? status = null,
        int page = 1,
        int pageSize = 10);

    Task<(List<Document> Documents, int TotalCount)> ListAsync(
        string? search,
        DocumentEntityType? entityId,
        bool? required,
        DocumentStatus? status,
        int page,
        int pageSize);

    Task<Document> GetByIdOrThrowAsync(long id);

    Task<Dictionary<long, Document>> GetByIdsAsync(IEnumerable<long> ids);

    Task<bool> ExistsByNameEnForEntityAsync(DocumentEntityType entityId, string nameEn);

    Task<bool> ExistsByNameArForEntityAsync(DocumentEntityType entityId, string nameAr);

    Task<bool> ExistsByNameEnExcludingIdAsync(long id, string nameEn);

    Task<bool> ExistsByNameArExcludingIdAsync(long id, string nameAr);
}
