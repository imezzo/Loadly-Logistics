using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripVehicleAssignmentSnapshotRepository : IRepository<TripVehicleAssignmentSnapshot>
{
    Task UpsertAsync(
        long tripId,
        string headPlateNo,
        string headPlateLetters,
        string? tailPlateNo,
        string? tailPlateLetters,
        long? addedBy);

    Task RemoveForTripAsync(long tripId);
}
