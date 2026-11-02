using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverLicenseTypes;

public class CreateDriverLicenseTypeAction
{
    private readonly IDriverLicenseTypeRepository _driverLicenseTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateDriverLicenseTypeAction(
        IDriverLicenseTypeRepository driverLicenseTypeRepository,
        ICurrentUserService currentUserService)
    {
        _driverLicenseTypeRepository = driverLicenseTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDriverLicenseTypeDto dto)
    {
        var driverLicenseType = new Common.Entities.DriverLicenseType
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        };
        await _driverLicenseTypeRepository.AddAsync(driverLicenseType);
        await _driverLicenseTypeRepository.SaveChangesAsync();
    }
}
