using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public class GetContractorDashboardStatisticsDto
{
    public long? ContractorId { get; set; }
}

public class ContractorDashboardStatisticsDto
{
    public long? ContractorId { get; set; }
    public string? ContractorFullName { get; set; }
    public ContractorDashboardTripsMetricDto Trips { get; set; } = new();
    public ContractorDashboardLoadlyProfileMarginDto LoadlyProfileMargin { get; set; } = new();
}

public class ContractorDashboardTripsMetricDto
{
    public int AllTime { get; set; }
    public int CurrentMonth { get; set; }
    public PeriodComparisonDto VsLastMonth { get; set; } = new();
}

public class ContractorDashboardLoadlyProfileMarginDto
{
    public decimal AllTime { get; set; }
    public decimal CurrentMonth { get; set; }
    public PeriodComparisonDto VsLastMonth { get; set; } = new();
}
