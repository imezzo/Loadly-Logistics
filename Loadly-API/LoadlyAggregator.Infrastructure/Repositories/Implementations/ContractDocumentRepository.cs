using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractDocumentRepository : Repository<ContractDocument>, IContractDocumentRepository
{
    public ContractDocumentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<ContractDocument?> GetByIdAsync(long id) =>
        _dbSet
            .Include(cd => cd.Document)
            .Include(cd => cd.Contract)
            .FirstOrDefaultAsync(cd => cd.Id == id);

    public async Task<IEnumerable<ContractDocument>> GetByContractIdAsync(long contractId) =>
        await _dbSet
            .Include(cd => cd.Document)
            .Where(cd => cd.ContractId == contractId)
            .ToListAsync();

    public Task<List<ContractDocument>> GetByContractIdsAsync(IEnumerable<long> contractIds) =>
        _dbSet
            .Include(cd => cd.Document)
            .Where(cd => contractIds.Contains(cd.ContractId))
            .ToListAsync();

    public async Task<IEnumerable<ContractDocument>> GetByDocumentIdAsync(long documentId) =>
        await _dbSet
            .Include(cd => cd.Contract)
            .Where(cd => cd.DocumentId == documentId)
            .ToListAsync();

    public async Task<(IEnumerable<ContractDocument> ContractDocuments, int TotalCount)> ListAsync(
        long? contractId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        DateTime? expiryDateFrom = null,
        DateTime? expiryDateTo = null,
        int page = 1,
        int pageSize = 10) =>
        (await _dbSet.AsQueryable()
            .Include(cd => cd.Document)
            .Include(cd => cd.Contract)
            .Where(cd => !contractId.HasValue || cd.ContractId == contractId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .Where(cd => !expiryDateFrom.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate >= expiryDateFrom.Value))
            .Where(cd => !expiryDateTo.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate <= expiryDateTo.Value))
            .OrderByDescending(cd => cd.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet.AsQueryable()
            .Where(cd => !contractId.HasValue || cd.ContractId == contractId.Value)
            .Where(cd => !documentId.HasValue || cd.DocumentId == documentId.Value)
            .Where(cd => !status.HasValue || cd.Status == status.Value)
            .Where(cd => !expiryDateFrom.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate >= expiryDateFrom.Value))
            .Where(cd => !expiryDateTo.HasValue || (cd.ExpiryDate.HasValue && cd.ExpiryDate <= expiryDateTo.Value))
            .CountAsync());

    public async Task<ContractDocument> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contract_documents.not_found_with_id", id);

    public override IQueryable<ContractDocument> Query() =>
        base.Query()
            .Include(cd => cd.Document)
            .Include(cd => cd.Contract);
}

