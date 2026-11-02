using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICompanyDocumentRepository : IRepository<CompanyDocument>
{
    Task<IEnumerable<CompanyDocument>> GetByCompanyIdAsync(long companyId);
    Task<IEnumerable<CompanyDocument>> GetByDocumentIdAsync(long documentId);
    Task<(IEnumerable<CompanyDocument> CompanyDocuments, int TotalCount)> ListAsync(
        long? companyId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        DateTime? expiryDateFrom = null,
        DateTime? expiryDateTo = null,
        int page = 1,
        int pageSize = 10);
    Task<CompanyDocument> GetByIdOrThrowAsync(long id);
}
