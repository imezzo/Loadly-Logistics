using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class TargetDto
{
    public long Id { get; set; }
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTargetDto
{
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
}

public class UpdateTargetDto
{
    public long Id { get; set; }
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
}

public class GetTargetDto
{
    public long Id { get; set; }
}

public class ListTargetsDto : BasePagedRequest
{
    public int? Year { get; set; }
    public TargetType? TargetType { get; set; }
}

public class DeleteTargetDto
{
    public long Id { get; set; }
}

public class GetMonthlyTargetsDto
{
    public int? Year { get; set; }
}

public class GetMonthlyYearMonthTargetsDto
{
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class MonthlyTargetDto
{
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public decimal TargetValue { get; set; }
    public TargetType TargetType { get; set; }
    public decimal AchievedValue { get; set; }
}
