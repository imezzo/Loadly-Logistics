using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;

public class UpdateDriverLicenseTypeAction
{
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;

    public UpdateDriverLicenseTypeAction(IDriverLicenseTypeRepository driverLicenseTypeRepository)
    {
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
    }

    public async Task ExecuteAsync(UpdateDriverLicenseTypeDto dto)
    {
        var driverLicenseType = await _driverLicenseTypeRepository.GetByIdOrThrowAsync(dto.Id);
        driverLicenseType.NameEn = dto.NameEn;
        driverLicenseType.NameAr = dto.NameAr;
        driverLicenseType.Status = dto.Status;
        await _driverLicenseTypeRepository.UpdateAsync(driverLicenseType);
        await _driverLicenseTypeRepository.SaveChangesAsync();
    }
}
