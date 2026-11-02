using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DocumentStatusRepository : Repository<LookupDocumentStatus>, IDocumentStatusRepository
{
    public DocumentStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<LookupDocumentStatus?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<LookupDocumentStatus?> GetByNameEnAsync(string nameEn) =>
        await _dbSet
            .FirstOrDefaultAsync(ds => ds.NameEn.ToLower() == nameEn.ToLower());

    public async Task<LookupDocumentStatus?> GetByNameArAsync(string nameAr) =>
        await _dbSet
            .FirstOrDefaultAsync(ds => ds.NameAr.ToLower() == nameAr.ToLower());

    public override async Task AddAsync(LookupDocumentStatus documentStatus) =>
        await base.AddAsync(documentStatus);

    public override void Remove(LookupDocumentStatus documentStatus) =>
        base.Remove(documentStatus);

    public override IQueryable<LookupDocumentStatus> Query() =>
        base.Query();

    public async Task<(List<LookupDocumentStatus> DocumentStatuses, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Where(ds => string.IsNullOrWhiteSpace(search) ||
                ds.NameEn.Contains(search!) ||
                ds.NameAr.Contains(search!))
            .OrderByDescending(ds => ds.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(ds => string.IsNullOrWhiteSpace(search) ||
                ds.NameEn.Contains(search!) ||
                ds.NameAr.Contains(search!))
            .CountAsync());

    public async Task<LookupDocumentStatus> GetByIdOrThrowAsync(long id) =>
        await _dbSet.FindAsync(id) ?? throw new NotFoundException("document_statuses.not_found_with_id", id);
}

