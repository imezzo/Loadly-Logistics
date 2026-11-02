using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.OperatingHours;

public class DeleteOperatingHoursAction
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;

    public DeleteOperatingHoursAction(IOperatingHoursRepository operatingHoursRepository)
    {
        _operatingHoursRepository = operatingHoursRepository;
    }

    public async Task ExecuteAsync(DeleteOperatingHoursDto dto)
    {
        _operatingHoursRepository.Remove(await _operatingHoursRepository.GetByIdOrThrowAsync(dto.Id));
        await _operatingHoursRepository.SaveChangesAsync();
    }
}
