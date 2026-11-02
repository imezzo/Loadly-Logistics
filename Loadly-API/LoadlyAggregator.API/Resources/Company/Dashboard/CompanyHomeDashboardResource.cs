using LoadlyAggregator.API.Resources.Admin.Dashboard;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Company.Dashboard;

public class CompanyHomeDashboardResource
{
    public CompanyHomeTotalTripsResource TotalTrips { get; set; } = new();
    public CompanyHomeTotalSpendingResource TotalSpending { get; set; } = new();
    public CompanyHomeTotalKmResource TotalKm { get; set; } = new();
    public IReadOnlyList<CompanyVehicleTypeSpendingResource> SpendingByVehicleType { get; set; } = new List<CompanyVehicleTypeSpendingResource>();
    public CompanyHomeInvoiceSummaryResource InvoiceSummary { get; set; } = new();

    public static CompanyHomeDashboardResource FromDto(CompanyHomeDashboardDto dto) => new()
    {
        TotalTrips = CompanyHomeTotalTripsResource.FromDto(dto.TotalTrips),
        TotalSpending = CompanyHomeTotalSpendingResource.FromDto(dto.TotalSpending),
        TotalKm = CompanyHomeTotalKmResource.FromDto(dto.TotalKm),
        SpendingByVehicleType = dto.SpendingByVehicleType
            .Select(CompanyVehicleTypeSpendingResource.FromDto)
            .ToList(),
        InvoiceSummary = CompanyHomeInvoiceSummaryResource.FromDto(dto.InvoiceSummary)
    };
}

public class CompanyHomeTotalTripsResource
{
    public int Total { get; set; }
    public PeriodComparisonResource VsYesterday { get; set; } = new();

    public static CompanyHomeTotalTripsResource FromDto(CompanyHomeTotalTripsDto dto) => new()
    {
        Total = dto.Total,
        VsYesterday = PeriodComparisonResource.FromDto(dto.VsYesterday)
    };
}

public class CompanyHomeTotalSpendingResource
{
    public decimal Amount { get; set; }
    public PeriodComparisonResource VsYesterday { get; set; } = new();

    public static CompanyHomeTotalSpendingResource FromDto(CompanyHomeTotalSpendingDto dto) => new()
    {
        Amount = dto.Amount,
        VsYesterday = PeriodComparisonResource.FromDto(dto.VsYesterday)
    };
}

public class CompanyHomeTotalKmResource
{
    public decimal Amount { get; set; }
    public PeriodComparisonResource VsYesterday { get; set; } = new();

    public static CompanyHomeTotalKmResource FromDto(CompanyHomeTotalKmDto dto) => new()
    {
        Amount = dto.Amount,
        VsYesterday = PeriodComparisonResource.FromDto(dto.VsYesterday)
    };
}

public class CompanyVehicleTypeSpendingResource
{
    public long VehicleTypeId { get; set; }
    public string VehicleTypeNameEn { get; set; } = string.Empty;
    public string VehicleTypeNameAr { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal SharePercent { get; set; }
    public PeriodComparisonResource VsLastWeek { get; set; } = new();

    public static CompanyVehicleTypeSpendingResource FromDto(CompanyVehicleTypeSpendingDto dto) => new()
    {
        VehicleTypeId = dto.VehicleTypeId,
        VehicleTypeNameEn = dto.VehicleTypeNameEn,
        VehicleTypeNameAr = dto.VehicleTypeNameAr,
        Amount = dto.Amount,
        SharePercent = dto.SharePercent,
        VsLastWeek = PeriodComparisonResource.FromDto(dto.VsLastWeek)
    };
}

public class CompanyHomeInvoiceSummaryResource
{
    public int PendingInvoices { get; set; }
    public int PaidThisMonth { get; set; }

    public static CompanyHomeInvoiceSummaryResource FromDto(CompanyHomeInvoiceSummaryDto dto) => new()
    {
        PendingInvoices = dto.PendingInvoices,
        PaidThisMonth = dto.PaidThisMonth
    };
}
