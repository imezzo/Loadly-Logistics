using Microsoft.Extensions.Caching.Memory;
using LoadlyAggregator.Domain.Admin.Actions.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public class CompanyGetHomeDashboardAction(
    ITripRepository tripRepository,
    IInvoiceRepository invoiceRepository,
    CompanyGetInvoiceStatisticsAction companyGetInvoiceStatisticsAction,
    IMemoryCache cache)
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(45);

    public async Task<CompanyHomeDashboardDto> ExecuteAsync(CompanyGetHomeDashboardDto request)
    {
        string cacheKey = CompanyDashboardDateRangeResolver.BuildHomeCacheKey(
            request.CompanyId,
            request.Year,
            request.LastMonth,
            request.LastWeek);

        if (cache.TryGetValue(cacheKey, out CompanyHomeDashboardDto? cached) && cached is not null)
        {
            return cached;
        }

        CompanyHomeDashboardDto result = await BuildAsync(request);
        cache.Set(cacheKey, result, CacheTtl);
        return result;
    }

    private async Task<CompanyHomeDashboardDto> BuildAsync(CompanyGetHomeDashboardDto request)
    {
        CompanyDashboardDateRangeResolver.Resolve(
            request.Year,
            request.LastMonth,
            request.LastWeek,
            out DateTime? from,
            out DateTime? to);

        if (from.HasValue && to.HasValue)
        {
            return await BuildFilteredAsync(request.CompanyId, from.Value, to.Value);
        }

        return await BuildDefaultAsync(request.CompanyId);
    }

    private async Task<CompanyHomeDashboardDto> BuildDefaultAsync(long companyId)
    {
        DateTime now = AppUtcTime.UtcNow(TimeProvider.System);
        DateTime today = AppUtcTime.UtcTodayAtMidnight(TimeProvider.System);

        // Sequential by design: a single scoped DbContext is shared and is not safe for concurrent access.
        return new CompanyHomeDashboardDto
        {
            TotalTrips = await BuildDefaultTotalTripsAsync(companyId, today),
            TotalSpending = await BuildDefaultTotalSpendingAsync(companyId, today),
            TotalKm = await BuildDefaultTotalKmAsync(companyId, today),
            SpendingByVehicleType = await BuildDefaultSpendingByVehicleTypeAsync(companyId, now),
            InvoiceSummary = await BuildDefaultInvoiceSummaryAsync(companyId, now)
        };
    }

    private async Task<CompanyHomeDashboardDto> BuildFilteredAsync(long companyId, DateTime from, DateTime to)
    {
        var (previousFrom, previousTo) = CompanyDashboardDateRangeResolver.GetPreviousPeriod(from, to);

        return new CompanyHomeDashboardDto
        {
            TotalTrips = await BuildFilteredTotalTripsAsync(companyId, from, to, previousFrom, previousTo),
            TotalSpending = await BuildFilteredTotalSpendingAsync(companyId, from, to, previousFrom, previousTo),
            TotalKm = await BuildFilteredTotalKmAsync(companyId, from, to, previousFrom, previousTo),
            SpendingByVehicleType = await BuildFilteredSpendingByVehicleTypeAsync(companyId, from, to, previousFrom, previousTo),
            InvoiceSummary = await BuildFilteredInvoiceSummaryAsync(companyId, from, to)
        };
    }

    private async Task<CompanyHomeTotalTripsDto> BuildDefaultTotalTripsAsync(long companyId, DateTime today)
    {
        var statusCounts = await tripRepository.GetTripStatusCountsForCompanyAsync(companyId);
        int todayCount = await tripRepository.GetCompanyTripCountInDateRangeAsync(companyId, today, today.AddDays(1));
        int yesterdayCount = await tripRepository.GetCompanyTripCountInDateRangeAsync(companyId, today.AddDays(-1), today);

        return new CompanyHomeTotalTripsDto
        {
            Total = statusCounts.Values.Sum(),
            VsYesterday = ComparisonCalculator.Compare(todayCount, yesterdayCount)
        };
    }

    private async Task<CompanyHomeTotalTripsDto> BuildFilteredTotalTripsAsync(
        long companyId,
        DateTime from,
        DateTime to,
        DateTime previousFrom,
        DateTime previousTo)
    {
        var statusCounts = await tripRepository.GetTripStatusCountsForCompanyAsync(companyId, from, to);
        int previousCount = await tripRepository.GetCompanyTripCountInDateRangeAsync(companyId, previousFrom, previousTo);

        return new CompanyHomeTotalTripsDto
        {
            Total = statusCounts.Values.Sum(),
            VsYesterday = ComparisonCalculator.Compare(statusCounts.Values.Sum(), previousCount)
        };
    }

    private async Task<CompanyHomeTotalSpendingDto> BuildDefaultTotalSpendingAsync(long companyId, DateTime today)
    {
        decimal total = await tripRepository.GetCompanyCompletedTripCostTotalAsync(companyId);
        decimal todayAmount = await tripRepository.GetCompanyCompletedTripCostTotalAsync(companyId, today, today.AddDays(1));
        decimal yesterdayAmount = await tripRepository.GetCompanyCompletedTripCostTotalAsync(companyId, today.AddDays(-1), today);

        return new CompanyHomeTotalSpendingDto
        {
            Amount = total,
            VsYesterday = ComparisonCalculator.Compare(todayAmount, yesterdayAmount)
        };
    }

    private async Task<CompanyHomeTotalSpendingDto> BuildFilteredTotalSpendingAsync(
        long companyId,
        DateTime from,
        DateTime to,
        DateTime previousFrom,
        DateTime previousTo)
    {
        decimal currentAmount = await tripRepository.GetCompanyCompletedTripCostTotalAsync(companyId, from, to);
        decimal previousAmount = await tripRepository.GetCompanyCompletedTripCostTotalAsync(companyId, previousFrom, previousTo);

        return new CompanyHomeTotalSpendingDto
        {
            Amount = currentAmount,
            VsYesterday = ComparisonCalculator.Compare(currentAmount, previousAmount)
        };
    }

    private async Task<CompanyHomeTotalKmDto> BuildDefaultTotalKmAsync(long companyId, DateTime today)
    {
        decimal total = await tripRepository.GetCompanyCompletedTripKilometersTotalAsync(companyId);
        decimal todayKm = await tripRepository.GetCompanyCompletedTripKilometersTotalAsync(companyId, today, today.AddDays(1));
        decimal yesterdayKm = await tripRepository.GetCompanyCompletedTripKilometersTotalAsync(companyId, today.AddDays(-1), today);

        return new CompanyHomeTotalKmDto
        {
            Amount = total,
            VsYesterday = ComparisonCalculator.Compare(todayKm, yesterdayKm)
        };
    }

    private async Task<CompanyHomeTotalKmDto> BuildFilteredTotalKmAsync(
        long companyId,
        DateTime from,
        DateTime to,
        DateTime previousFrom,
        DateTime previousTo)
    {
        decimal currentKm = await tripRepository.GetCompanyCompletedTripKilometersTotalAsync(companyId, from, to);
        decimal previousKm = await tripRepository.GetCompanyCompletedTripKilometersTotalAsync(companyId, previousFrom, previousTo);

        return new CompanyHomeTotalKmDto
        {
            Amount = currentKm,
            VsYesterday = ComparisonCalculator.Compare(currentKm, previousKm)
        };
    }

    private async Task<IReadOnlyList<CompanyVehicleTypeSpendingDto>> BuildDefaultSpendingByVehicleTypeAsync(long companyId, DateTime now)
    {
        var (thisWeekStart, thisWeekEnd) = AppUtcTime.UtcSaturdayWeekRange(now);
        var (lastWeekStart, lastWeekEnd) = AppUtcTime.UtcPreviousSaturdayWeekRange(now);

        var allTime = await tripRepository.GetCompanyCompletedTripCostByVehicleTypeAsync(companyId);
        var thisWeek = await tripRepository.GetCompanyCompletedTripCostByVehicleTypeAsync(companyId, thisWeekStart, thisWeekEnd);
        var lastWeek = await tripRepository.GetCompanyCompletedTripCostByVehicleTypeAsync(companyId, lastWeekStart, lastWeekEnd);

        var thisWeekByType = thisWeek.ToDictionary(x => x.VehicleTypeId, x => x.Amount);
        var lastWeekByType = lastWeek.ToDictionary(x => x.VehicleTypeId, x => x.Amount);

        decimal totalAmount = allTime.Sum(x => x.Amount);

        return allTime
            .OrderByDescending(x => x.Amount)
            .Select(x => new CompanyVehicleTypeSpendingDto
            {
                VehicleTypeId = x.VehicleTypeId,
                VehicleTypeNameEn = x.NameEn,
                VehicleTypeNameAr = x.NameAr,
                Amount = x.Amount,
                SharePercent = totalAmount == 0m
                    ? 0m
                    : DashboardTripMetrics.Round(x.Amount * 100m / totalAmount),
                VsLastWeek = ComparisonCalculator.Compare(
                    thisWeekByType.GetValueOrDefault(x.VehicleTypeId),
                    lastWeekByType.GetValueOrDefault(x.VehicleTypeId))
            })
            .ToList();
    }

    private async Task<IReadOnlyList<CompanyVehicleTypeSpendingDto>> BuildFilteredSpendingByVehicleTypeAsync(
        long companyId,
        DateTime from,
        DateTime to,
        DateTime previousFrom,
        DateTime previousTo)
    {
        var current = await tripRepository.GetCompanyCompletedTripCostByVehicleTypeAsync(companyId, from, to);
        var previous = await tripRepository.GetCompanyCompletedTripCostByVehicleTypeAsync(companyId, previousFrom, previousTo);

        var previousByType = previous.ToDictionary(x => x.VehicleTypeId, x => x.Amount);
        decimal totalAmount = current.Sum(x => x.Amount);

        return current
            .OrderByDescending(x => x.Amount)
            .Select(x => new CompanyVehicleTypeSpendingDto
            {
                VehicleTypeId = x.VehicleTypeId,
                VehicleTypeNameEn = x.NameEn,
                VehicleTypeNameAr = x.NameAr,
                Amount = x.Amount,
                SharePercent = totalAmount == 0m
                    ? 0m
                    : DashboardTripMetrics.Round(x.Amount * 100m / totalAmount),
                VsLastWeek = ComparisonCalculator.Compare(
                    x.Amount,
                    previousByType.GetValueOrDefault(x.VehicleTypeId))
            })
            .ToList();
    }

    private async Task<CompanyHomeInvoiceSummaryDto> BuildDefaultInvoiceSummaryAsync(long companyId, DateTime now)
    {
        var invoiceStatistics = await companyGetInvoiceStatisticsAction.ExecuteAsync(new CompanyGetInvoiceStatisticsDto { CompanyId = companyId });

        DateTime monthStart = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        int paidThisMonth = await invoiceRepository.GetCompanyPaidInvoicesCountInRangeAsync(companyId, monthStart, monthStart.AddMonths(1));

        return new CompanyHomeInvoiceSummaryDto
        {
            PendingInvoices = invoiceStatistics.UnpaidInvoicesCount,
            PaidThisMonth = paidThisMonth
        };
    }

    private async Task<CompanyHomeInvoiceSummaryDto> BuildFilteredInvoiceSummaryAsync(long companyId, DateTime from, DateTime to)
    {
        var invoiceStatistics = await companyGetInvoiceStatisticsAction.ExecuteAsync(new CompanyGetInvoiceStatisticsDto { CompanyId = companyId });
        int paidInPeriod = await invoiceRepository.GetCompanyPaidInvoicesCountInRangeAsync(companyId, from, to);

        return new CompanyHomeInvoiceSummaryDto
        {
            PendingInvoices = invoiceStatistics.UnpaidInvoicesCount,
            PaidThisMonth = paidInPeriod
        };
    }
}
