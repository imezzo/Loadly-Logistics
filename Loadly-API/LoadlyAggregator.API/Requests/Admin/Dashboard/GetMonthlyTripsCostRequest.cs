using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetMonthlyTripsCostRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }
    public int? Month { get; set; }

    public GetMonthlyTripsCostDto FromRequest() => new()
    {
        Year = Year ?? AppUtcTime.UtcCalendarYear(TimeProvider.System),
        Month = Month
    };
}
