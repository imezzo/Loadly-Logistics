using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.Domain.Company.DTOs.Dashboard;

public class CompanyHomeDashboardDto
{
    public CompanyHomeTotalTripsDto TotalTrips { get; set; } = new();
    public CompanyHomeTotalSpendingDto TotalSpending { get; set; } = new();
    public CompanyHomeTotalKmDto TotalKm { get; set; } = new();
    public IReadOnlyList<CompanyVehicleTypeSpendingDto> SpendingByVehicleType { get; set; } = new List<CompanyVehicleTypeSpendingDto>();
    public CompanyHomeInvoiceSummaryDto InvoiceSummary { get; set; } = new();
}

public class CompanyHomeTotalTripsDto
{
    public int Total { get; set; }
    public PeriodComparisonDto VsYesterday { get; set; } = new();
}

public class CompanyHomeTotalSpendingDto
{
    public decimal Amount { get; set; }
    public PeriodComparisonDto VsYesterday { get; set; } = new();
}

public class CompanyHomeTotalKmDto
{
    public decimal Amount { get; set; }
    public PeriodComparisonDto VsYesterday { get; set; } = new();
}

public class CompanyVehicleTypeSpendingDto
{
    public long VehicleTypeId { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal SharePercent { get; set; }
    public PeriodComparisonDto VsLastWeek { get; set; } = new();
}

public class CompanyHomeInvoiceSummaryDto
{
    public int PendingInvoices { get; set; }
    public int PaidThisMonth { get; set; }
}

public class CompanyGetHomeDashboardDto
{
    public long CompanyId { get; set; }
    public int? Year { get; set; }
    public bool LastMonth { get; set; }
    public bool LastWeek { get; set; }
}
