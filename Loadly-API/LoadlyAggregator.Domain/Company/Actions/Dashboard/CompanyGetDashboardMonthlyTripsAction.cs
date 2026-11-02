using System.Linq;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public class CompanyGetDashboardMonthlyTripsAction(ITripRepository tripRepository)
{
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<ListingDto<CompanyMonthlyTripKilometersDto>> ExecuteAsync(CompanyGetDashboardMonthlyTripsDto dto)
    {
        var monthly = await _tripRepository.GetMonthlyCompletedTripKilometerAggregatesForCompanyAsync(
            dto.CompanyId,
            dto.Year,
            dto.Month);
        var byMonth = monthly.ToDictionary(x => x.Month, x => (x.TripsCount, x.TotalKilometers));

        IEnumerable<int> months = dto.Month.HasValue ? new[] { dto.Month.Value } : Enumerable.Range(1, 12);
        List<CompanyMonthlyTripKilometersDto> data = months.Select(m => new CompanyMonthlyTripKilometersDto
        {
            Month = m,
            TotalTrips = byMonth.GetValueOrDefault(m).TripsCount,
            TotalKilometers = byMonth.GetValueOrDefault(m).TotalKilometers
        }).ToList();

        return new ListingDto<CompanyMonthlyTripKilometersDto> { Data = data };
    }
}
