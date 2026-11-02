using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Services;

public class TripVehicleAssignmentSnapshotRecorder(ITripVehicleAssignmentSnapshotRepository repository)
{
    private readonly ITripVehicleAssignmentSnapshotRepository _repository = repository;

    public async Task SyncForTripAsync(
        long tripId,
        Vehicle? headVehicle,
        long? tailVehicleId,
        Vehicle? tailVehicle,
        bool freezeSnapshot,
        long? addedBy = null)
    {
        if (freezeSnapshot)
            return;

        if (headVehicle is null && !tailVehicleId.HasValue)
        {
            await _repository.RemoveForTripAsync(tripId);
            return;
        }

        string headPlateNo = headVehicle?.PlateNo ?? string.Empty;
        string headPlateLetters = headVehicle?.PlateLetters ?? string.Empty;
        string? tailPlateNo = null;
        string? tailPlateLetters = null;

        if (tailVehicle != null)
        {
            tailPlateNo = tailVehicle.PlateNo;
            tailPlateLetters = tailVehicle.PlateLetters;
        }
        else if (headVehicle?.TailVehicles.FirstOrDefault() is { } embeddedTail)
        {
            tailPlateNo = embeddedTail.PlateNo;
            tailPlateLetters = embeddedTail.PlateLetters;
        }

        await _repository.UpsertAsync(
            tripId,
            headPlateNo,
            headPlateLetters,
            tailPlateNo,
            tailPlateLetters,
            addedBy);
    }
}
