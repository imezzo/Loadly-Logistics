using System.Collections.Generic;
using System.Threading.Tasks;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractDocumentRepository : IRepository<ContractDocument>
{
    Task<IEnumerable<ContractDocument>> GetByContractIdAsync(long contractId);
    Task<List<ContractDocument>> GetByContractIdsAsync(IEnumerable<long> contractIds);
    Task<IEnumerable<ContractDocument>> GetByDocumentIdAsync(long documentId);
    Task<(IEnumerable<ContractDocument> ContractDocuments, int TotalCount)> ListAsync(
        long? contractId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        DateTime? expiryDateFrom = null,
        DateTime? expiryDateTo = null,
        int page = 1,
        int pageSize = 10);
    Task<ContractDocument> GetByIdOrThrowAsync(long id);
}
