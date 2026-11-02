using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetMonthlyTargetsRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }

    public GetMonthlyTargetsDto FromRequest() => new()
    {
        Year = Year
    };
}
