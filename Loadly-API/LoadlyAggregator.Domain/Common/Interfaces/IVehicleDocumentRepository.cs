using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleDocumentRepository : IRepository<VehicleDocument>
{
    Task<(List<VehicleDocument> VehicleDocuments, int TotalCount)> ListAsync(
        long? vehicleId = null,
        long? documentId = null,
        DocumentVerificationStatus? status = null,
        int page = 1,
        int pageSize = 10);

    Task<VehicleDocument> GetByIdOrThrowAsync(long id);
    Task<IEnumerable<VehicleDocument>> GetByVehicleIdAsync(long vehicleId); // <-- added

}
