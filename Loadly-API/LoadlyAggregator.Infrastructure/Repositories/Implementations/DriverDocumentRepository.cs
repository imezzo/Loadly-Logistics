using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DriverDocumentRepository : Repository<DriverDocument>, IDriverDocumentRepository
{
    public DriverDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<DriverDocument?> GetByIdAsync(long id) =>
        _dbSet
            .Include(dd => dd.Document)
            .Include(dd => dd.Driver)
            .FirstOrDefaultAsync(dd => dd.Id == id);

    public async Task<(IEnumerable<DriverDocument> DriverDocuments, int TotalCount)> ListAsync(
        long? driverId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .AsNoTracking()
            .Include(dd => dd.Document)
            .Include(dd => dd.Driver)
            .Where(dd => !driverId.HasValue || dd.DriverId == driverId.Value)
            .Where(dd => !documentId.HasValue || dd.DocumentId == documentId.Value)
            .Where(dd => !status.HasValue || dd.Status == status.Value)
            .OrderByDescending(dd => dd.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Include(dd => dd.Document)
            .Include(dd => dd.Driver)
            .Where(dd => !driverId.HasValue || dd.DriverId == driverId.Value)
            .Where(dd => !documentId.HasValue || dd.DocumentId == documentId.Value)
            .Where(dd => !status.HasValue || dd.Status == status.Value)
            .CountAsync());

    public async Task<DriverDocument> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("driver_documents.not_found_with_id", id);

    public override IQueryable<DriverDocument> Query() =>
        base.Query()
            .Include(dd => dd.Document)
            .Include(dd => dd.Driver);
}
