using System.Diagnostics;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Services;

/// <summary>Records driver/tail assignment changes (prev → new). All writers use the same delta shape.</summary>
public class VehicleAssignmentHistoryRecorder(IVehicleAssignmentHistoryRepository repository)
{
    private readonly IVehicleAssignmentHistoryRepository _repository = repository;

    public static long? ResolveTailVehicleId(Vehicle head, IEnumerable<Vehicle> transactionVehicles) =>
        transactionVehicles.FirstOrDefault(v => v.HeadVehicleId == head.Id)?.Id;

    public Task RecordChangeAsync(
        long vehicleId,
        VehicleAssignmentHistorySource source,
        long? previousDriverId,
        long? previousTailVehicleId,
        long? newDriverId,
        long? newTailVehicleId,
        long? addedBy)
    {
        if (previousDriverId == newDriverId && previousTailVehicleId == newTailVehicleId)
            return Task.CompletedTask;

        return _repository.AddChangeAsync(
            vehicleId,
            source,
            previousDriverId,
            previousTailVehicleId,
            newDriverId,
            newTailVehicleId,
            addedBy);
    }
}
