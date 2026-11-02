using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Services;

public class VehiclePlateAliasRecorder(IVehiclePlateAliasRepository repository)
{
    private readonly IVehiclePlateAliasRepository _repository = repository;

    public Task RecordSupersededPlateAsync(long vehicleId, string plateNo, string plateLetters, long? addedBy = null)
    {
        if (string.IsNullOrWhiteSpace(plateNo) || string.IsNullOrWhiteSpace(plateLetters))
            return Task.CompletedTask;

        return _repository.AddIfNotExistsAsync(vehicleId, plateNo.Trim(), plateLetters.Trim(), addedBy);
    }
}
