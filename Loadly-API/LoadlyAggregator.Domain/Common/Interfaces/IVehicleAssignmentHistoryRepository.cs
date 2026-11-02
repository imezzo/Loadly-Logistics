using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleAssignmentHistoryRepository : IRepository<VehicleAssignmentHistory>
{
    Task AddChangeAsync(
        long vehicleId,
        VehicleAssignmentHistorySource source,
        long? previousDriverId,
        long? previousTailVehicleId,
        long? newDriverId,
        long? newTailVehicleId,
        long? addedBy);

    Task<(List<VehicleAssignmentHistory> Items, int TotalCount)> ListAsync(
        long vehicleId,
        VehicleAssignmentHistorySource? source,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize);
}
