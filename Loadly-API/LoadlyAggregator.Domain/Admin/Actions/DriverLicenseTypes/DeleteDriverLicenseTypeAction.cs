using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;

public class DeleteDriverLicenseTypeAction
{
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;

    public DeleteDriverLicenseTypeAction(IDriverLicenseTypeRepository driverLicenseTypeRepository)
    {
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
    }

    public async Task ExecuteAsync(DeleteDriverLicenseTypeDto dto)
    {
        var driverLicenseType = await _driverLicenseTypeRepository.GetByIdOrThrowAsync(dto.Id);
        _driverLicenseTypeRepository.Remove(driverLicenseType);
        await _driverLicenseTypeRepository.SaveChangesAsync();
    }
}
