using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CompanyDocumentRepository : Repository<CompanyDocument>, ICompanyDocumentRepository
{
    public CompanyDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<CompanyDocument?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cd => cd.Document)
            .Include(cd => cd.Company)
            .FirstOrDefaultAsync(cd => cd.Id == id);

    public async Task<IEnumerable<CompanyDocument>> GetByCompanyIdAsync(long companyId) =>
        await _dbSet.Include(cd => cd.Document).Where(cd => cd.CompanyId == companyId).ToListAsync();

    public async Task<IEnumerable<CompanyDocument>> GetByDocumentIdAsync(long documentId) =>
        await _dbSet.Include(cd => cd.Company).Where(cd => cd.DocumentId == documentId).ToListAsync();

    public async Task<(IEnumerable<CompanyDocument> CompanyDocuments, int TotalCount)> ListAsync(
        long? companyId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        DateTime? expiryDateFrom = null,
        DateTime? expiryDateTo = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .Include(cd => cd.Document)
            .Include(cd => cd.Company)
            .Where(cd => !companyId.HasValue || cd.CompanyId == companyId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .Where(cd => !expiryDateFrom.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate >= expiryDateFrom.Value))
            .Where(cd => !expiryDateTo.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate <= expiryDateTo.Value))
            .OrderByDescending(cd => cd.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Include(cd => cd.Document)
            .Include(cd => cd.Company)
            .Where(cd => !companyId.HasValue || cd.CompanyId == companyId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .Where(cd => !expiryDateFrom.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate >= expiryDateFrom.Value))
            .Where(cd => !expiryDateTo.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate <= expiryDateTo.Value))
            .CountAsync());

    public async Task<CompanyDocument> GetByIdOrThrowAsync(long id) =>
        await _dbSet
            .Include(cd => cd.Document)
            .Include(cd => cd.Company)
            .FirstOrDefaultAsync(cd => cd.Id == id)
        ?? throw new NotFoundException("company_documents.not_found_with_id", id);

    public override IQueryable<CompanyDocument> Query() =>
        base.Query().Include(cd => cd.Document).Include(cd => cd.Company);
}

