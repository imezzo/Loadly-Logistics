using System.Collections.Generic;
using System.Threading.Tasks;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripDocumentRepository : IRepository<TripDocument>
{
    Task<IEnumerable<TripDocument>> GetByTripIdAsync(long tripId);
    Task<IEnumerable<TripDocument>> GetByDocumentIdAsync(long documentId);

    Task<TripDocument?> GetCurrentByTripAndDocumentAsync(long tripId, long documentId);

    Task<(IEnumerable<TripDocument> TripDocuments, int TotalCount)> ListAsync(
        long? tripId = null,
        long? documentId = null,
        CollectorDocumentStatus? status = null,
        int page = 1,
        int pageSize = 10);
    Task<TripDocument> GetByIdOrThrowAsync(long id);
}
