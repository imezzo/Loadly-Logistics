using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDriverDocumentRepository : IRepository<DriverDocument>
{
    Task<(IEnumerable<DriverDocument> DriverDocuments, int TotalCount)> ListAsync(
        long? driverId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10);
    Task<DriverDocument> GetByIdOrThrowAsync(long id);
}
