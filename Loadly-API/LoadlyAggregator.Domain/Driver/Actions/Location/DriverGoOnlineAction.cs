using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Location;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Driver.Actions.Location;

public class DriverGoOnlineAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverLocationRepository _driverLocationRepository;
    private readonly TimeProvider _timeProvider;

    public DriverGoOnlineAction(
        IDriverRepository driverRepository,
        IDriverLocationRepository driverLocationRepository,
        TimeProvider timeProvider)
    {
        _driverRepository = driverRepository;
        _driverLocationRepository = driverLocationRepository;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(DriverGoOnlineDto dto)
    {
        await ValidateDriverExistsAsync(dto.DriverId);
        await SaveDriverLocationAsync(dto);
        await _driverRepository.UpdateWorkingStatusAsync(dto.DriverId, DriverWorkingStatus.Available);
        await _driverRepository.SaveChangesAsync();
    }

    private async Task ValidateDriverExistsAsync(long driverId)
    {
        _ = await _driverRepository.GetByIdOrThrowAsync(driverId);
    }

    private async Task SaveDriverLocationAsync(DriverGoOnlineDto dto)
    {
        await _driverLocationRepository.UpsertAsync(new Domain.Common.Entities.DriverLocation
        {
            DriverId = dto.DriverId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            RecordedAt = AppUtcTime.UtcNow(_timeProvider)
        });
    }
}
