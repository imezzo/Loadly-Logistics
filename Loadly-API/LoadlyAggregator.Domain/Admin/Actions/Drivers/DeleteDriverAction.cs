using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class DeleteDriverAction
{
    private readonly IDriverRepository _driverRepository;

    public DeleteDriverAction(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task ExecuteAsync(DeleteDriverDto dto)
    {
        var driver = await _driverRepository.GetByIdOrThrowAsync(dto.Id);
        _driverRepository.Remove(driver);
        await _driverRepository.SaveChangesAsync();
    }
}
