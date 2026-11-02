using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class TargetResource
{
    public long Id { get; set; }
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static TargetResource FromDto(TargetDto dto) => new()
    {
        Id = dto.Id,
        Month = dto.Month,
        Year = dto.Year,
        TargetType = dto.TargetType,
        TargetValue = dto.TargetValue,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
