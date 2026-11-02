using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractorDocumentRepository : Repository<ContractorDocument>, IContractorDocumentRepository
{
    public ContractorDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<ContractorDocument?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cd => cd.Document)
            .FirstOrDefaultAsync(cd => cd.Id == id);

    public async Task<IEnumerable<ContractorDocument>> GetByContractorIdAsync(long contractorId) =>
        await _dbSet
            .Include(cd => cd.Document)
            .Where(cd => cd.ContractorId == contractorId)
            .ToListAsync();

    public async Task<(IEnumerable<ContractorDocument> ContractorDocuments, int TotalCount)> ListAsync(
        long? contractorId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .Include(cd => cd.Document)
            .Where(cd => !contractorId.HasValue || cd.ContractorId == contractorId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .OrderByDescending(cd => cd.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(cd => !contractorId.HasValue || cd.ContractorId == contractorId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .CountAsync());

    public async Task<ContractorDocument> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contractor_documents.not_found_with_id", id);
}
