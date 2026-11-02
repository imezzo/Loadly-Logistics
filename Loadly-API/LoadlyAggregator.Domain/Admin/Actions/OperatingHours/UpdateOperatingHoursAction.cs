using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.OperatingHours;

public class UpdateOperatingHoursAction
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;

    public UpdateOperatingHoursAction(IOperatingHoursRepository operatingHoursRepository)
    {
        _operatingHoursRepository = operatingHoursRepository;
    }

    public async Task ExecuteAsync(UpdateOperatingHoursDto dto)
    {
        var operatingHours = await _operatingHoursRepository.GetByIdOrThrowAsync(dto.Id);

        operatingHours.NameEn = dto.NameEn;
        operatingHours.NameAr = dto.NameAr;
        await _operatingHoursRepository.UpdateAsync(operatingHours);
        await _operatingHoursRepository.SaveChangesAsync();
    }
}
