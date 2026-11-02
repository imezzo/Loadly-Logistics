using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class UpdateTargetAction(ITargetRepository targetRepository)
{
    private readonly ITargetRepository _targetRepository = targetRepository;

    public async Task ExecuteAsync(UpdateTargetDto dto)
    {
        var conflicting = await _targetRepository.GetByMonthYearAndTypeAsync(dto.Month, dto.Year, dto.TargetType, dto.Id);
        if (conflicting != null)
            throw new ValidationException("targets.already_exists_for_month_year");

        var target = await _targetRepository.GetByIdOrThrowAsync(dto.Id);
        target.Month = dto.Month;
        target.Year = dto.Year;
        target.TargetType = dto.TargetType;
        target.TargetValue = dto.TargetValue;
        await _targetRepository.UpdateAsync(target);
        await _targetRepository.SaveChangesAsync();
    }
}
