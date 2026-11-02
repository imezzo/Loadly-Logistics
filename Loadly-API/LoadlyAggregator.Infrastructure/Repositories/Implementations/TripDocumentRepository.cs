using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class TripDocumentRepository : Repository<TripDocument>, ITripDocumentRepository
{
    public TripDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<TripDocument?> GetByIdAsync(long id) =>
        _dbSet
            .Include(td => td.Document)
            .Include(td => td.Trip)
            .FirstOrDefaultAsync(td => td.Id == id);

    public async Task<IEnumerable<TripDocument>> GetByTripIdAsync(long tripId) =>
        await _dbSet
            .Include(td => td.Document)
            .Where(td => td.TripId == tripId)
            .ToListAsync();

    public async Task<IEnumerable<TripDocument>> GetByDocumentIdAsync(long documentId) =>
        await _dbSet
            .Include(td => td.Trip)
            .Where(td => td.DocumentId == documentId)
            .ToListAsync();

    public Task<TripDocument?> GetCurrentByTripAndDocumentAsync(long tripId, long documentId) =>
        _dbSet
            .Include(td => td.Document)
            .Where(td => td.TripId == tripId && td.DocumentId == documentId)
            .OrderByDescending(td => td.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<(IEnumerable<TripDocument> TripDocuments, int TotalCount)> ListAsync(
        long? tripId = null,
        long? documentId = null,
        CollectorDocumentStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Include(td => td.Document)
            .Include(td => td.Trip)
            .Where(td => !tripId.HasValue || td.TripId == tripId.Value)
            .Where(td => !documentId.HasValue || td.DocumentId == documentId.Value)
            .Where(td => !status.HasValue || td.Status == status.Value)
            .OrderByDescending(td => td.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(td => !tripId.HasValue || td.TripId == tripId.Value)
            .Where(td => !documentId.HasValue || td.DocumentId == documentId.Value)
            .Where(td => !status.HasValue || td.Status == status.Value)
            .CountAsync());

    public async Task<TripDocument> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("trip_documents.not_found_with_id", id);

    public override IQueryable<TripDocument> Query() =>
        base.Query()
            .Include(td => td.Document)
            .Include(td => td.Trip);
}
