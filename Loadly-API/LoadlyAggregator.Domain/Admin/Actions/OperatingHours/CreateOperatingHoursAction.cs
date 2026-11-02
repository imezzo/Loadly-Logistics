using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.OperatingHours;

public class CreateOperatingHoursAction
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateOperatingHoursAction(
        IOperatingHoursRepository operatingHoursRepository,
        ICurrentUserService currentUserService)
    {
        _operatingHoursRepository = operatingHoursRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateOperatingHoursDto dto)
    {
        await _operatingHoursRepository.AddAsync(new OperatingHour
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _operatingHoursRepository.SaveChangesAsync();
    }
}
