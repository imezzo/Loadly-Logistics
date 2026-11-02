using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractorDocumentRepository : IRepository<ContractorDocument>
{
    Task<IEnumerable<ContractorDocument>> GetByContractorIdAsync(long contractorId);

    Task<(IEnumerable<ContractorDocument> ContractorDocuments, int TotalCount)> ListAsync(
        long? contractorId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10);

    Task<ContractorDocument> GetByIdOrThrowAsync(long id);
}
