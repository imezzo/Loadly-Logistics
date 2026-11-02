using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class GetDriverWorkingStatisticsAction(IDriverRepository driverRepository)
{
    private readonly IDriverRepository _driverRepository = driverRepository;

    public async Task<DriversWorkingStatisticsDto> ExecuteAsync()
    {
        IReadOnlyDictionary<DriverWorkingStatus, int> counts = await _driverRepository.GetWorkingStatusCountsAsync();

        return new DriversWorkingStatisticsDto
        {
            Available = counts.GetValueOrDefault(DriverWorkingStatus.Available),
            Busy = counts.GetValueOrDefault(DriverWorkingStatus.Busy),
            OnBreak = counts.GetValueOrDefault(DriverWorkingStatus.OnBreak),
            Offline = counts.GetValueOrDefault(DriverWorkingStatus.Offline),
            OffDuty = counts.GetValueOrDefault(DriverWorkingStatus.OffDuty),
            TotalDrivers = counts.Values.Sum()
        };
    }
}
