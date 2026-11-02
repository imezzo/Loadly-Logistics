using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetMonthlyCostTargetsRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }
    public int? Month { get; set; }

    public GetMonthlyYearMonthTargetsDto FromRequest() => new()
    {
        Year = Year,
        Month = Month
    };
}
