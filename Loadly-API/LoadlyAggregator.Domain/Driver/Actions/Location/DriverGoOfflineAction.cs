using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Location;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Location;

public class DriverGoOfflineAction
{
    private readonly IDriverRepository _driverRepository;

    public DriverGoOfflineAction(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task ExecuteAsync(DriverGoOfflineDto dto)
    {
        await UpdateDriverTimestampAsync(dto.DriverId);
        await _driverRepository.UpdateWorkingStatusAsync(dto.DriverId, DriverWorkingStatus.Offline);
        await _driverRepository.SaveChangesAsync();
    }

    private async Task UpdateDriverTimestampAsync(long driverId)
    {
        var driver = await _driverRepository.GetByIdOrThrowAsync(driverId);
        await _driverRepository.UpdateAsync(driver);
    }
}
