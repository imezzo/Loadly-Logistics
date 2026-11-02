using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Document?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<Document>> GetByEntityIdAsync(DocumentEntityType entityId) =>
        await _dbSet
            .Where(d => d.EntityId == entityId)
            .ToListAsync();

    public async Task<IEnumerable<Document>> GetRequiredActiveByEntityAsync(DocumentEntityType entityId) =>
        await _dbSet
            .Where(d => d.EntityId == entityId && d.Required && d.Status == DocumentStatus.ACTIVE)
            .ToListAsync();

    public async Task<(IEnumerable<Document> Documents, int TotalCount)> SearchAsync(
        string? search = null,
        DocumentEntityType? entityId = null,
        bool? required = null,
        DocumentStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        (await BuildSearchQuery(search, entityId, required, status)
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, entityId, required, status).CountAsync());

    private IQueryable<Document> BuildSearchQuery(string? search, DocumentEntityType? entityId, bool? required, DocumentStatus? status) =>
        _dbSet.AsQueryable()
            .Where(d => string.IsNullOrWhiteSpace(search) || d.NameEn.Contains(search) || (d.NameAr != null && d.NameAr.Contains(search)))
            .Where(d => !entityId.HasValue || d.EntityId == entityId.Value)
            .Where(d => !required.HasValue || d.Required == required.Value)
            .Where(d => !status.HasValue || d.Status == status.Value);

    public async Task<(List<Document> Documents, int TotalCount)> ListAsync(
        string? search,
        DocumentEntityType? entityId,
        bool? required,
        DocumentStatus? status,
        int page,
        int pageSize) =>
        (await BuildSearchQuery(search, entityId, required, status)
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await BuildSearchQuery(search, entityId, required, status).CountAsync());

    public async Task<Document> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("documents.not_found_with_id", id);

    public async Task<Dictionary<long, Document>> GetByIdsAsync(IEnumerable<long> ids) =>
        await _dbSet
            .Where(d => ids.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id);

    public async Task<bool> ExistsByNameEnForEntityAsync(DocumentEntityType entityId, string nameEn) =>
        await _dbSet.AnyAsync(d => d.EntityId == entityId && d.NameEn == nameEn);

    public async Task<bool> ExistsByNameArForEntityAsync(DocumentEntityType entityId, string nameAr) =>
        await _dbSet.AnyAsync(d => d.EntityId == entityId && d.NameAr == nameAr);

    public async Task<bool> ExistsByNameEnExcludingIdAsync(long id, string nameEn) =>
        await _dbSet.AnyAsync(d => d.Id != id && d.NameEn == nameEn
            && d.EntityId == _dbSet.Where(x => x.Id == id).Select(x => x.EntityId).FirstOrDefault());

    public async Task<bool> ExistsByNameArExcludingIdAsync(long id, string nameAr) =>
        await _dbSet.AnyAsync(d => d.Id != id && d.NameAr == nameAr
            && d.EntityId == _dbSet.Where(x => x.Id == id).Select(x => x.EntityId).FirstOrDefault());
}
