using LoadlyAggregator.API.Resources.Admin.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Contractors;

public class ContractorDashboardStatisticsResource
{
    public long? ContractorId { get; set; }
    public string? ContractorFullName { get; set; }
    public ContractorDashboardTripsMetricResource Trips { get; set; } = new();
    public ContractorDashboardLoadlyProfileMarginResource LoadlyProfileMargin { get; set; } = new();
}

public class ContractorDashboardTripsMetricResource
{
    public int AllTime { get; set; }
    public int CurrentMonth { get; set; }
    public PeriodComparisonResource VsLastMonth { get; set; } = new();
}

public class ContractorDashboardLoadlyProfileMarginResource
{
    public decimal AllTime { get; set; }
    public decimal CurrentMonth { get; set; }
    public PeriodComparisonResource VsLastMonth { get; set; } = new();
}
