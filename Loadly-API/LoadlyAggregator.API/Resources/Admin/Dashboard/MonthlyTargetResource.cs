using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class MonthlyTargetResource
{
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public decimal TargetValue { get; set; }
    public TargetType TargetType { get; set; }
    public decimal AchievedValue { get; set; }

    public static MonthlyTargetResource FromDto(MonthlyTargetDto dto) => new()
    {
        Month = dto.Month,
        Year = dto.Year,
        TargetValue = dto.TargetValue,
        TargetType = dto.TargetType,
        AchievedValue = dto.AchievedValue
    };
}
