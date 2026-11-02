using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class PeriodComparisonDto
{
    public decimal Current { get; set; }
    public decimal Previous { get; set; }
    public decimal Delta { get; set; }
    public decimal PercentChange { get; set; }
    public TrendDirection Direction { get; set; }
}
