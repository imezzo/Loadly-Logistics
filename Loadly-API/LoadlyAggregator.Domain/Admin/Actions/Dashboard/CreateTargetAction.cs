using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class CreateTargetAction(ITargetRepository targetRepository)
{
    private readonly ITargetRepository _targetRepository = targetRepository;

    public async Task ExecuteAsync(CreateTargetDto dto)
    {
        if (await _targetRepository.GetByMonthYearAndTypeAsync(dto.Month, dto.Year, dto.TargetType) != null)
            throw new ValidationException("targets.already_exists_for_month_year");

        await _targetRepository.AddAsync(new Target
        {
            Month = dto.Month,
            Year = dto.Year,
            TargetType = dto.TargetType,
            TargetValue = dto.TargetValue
        });
        await _targetRepository.SaveChangesAsync();
    }
}
