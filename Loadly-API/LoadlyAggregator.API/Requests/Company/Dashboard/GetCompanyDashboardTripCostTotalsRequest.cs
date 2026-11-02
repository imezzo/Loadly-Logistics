using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Company.Dashboard;

public class GetCompanyDashboardTripCostTotalsRequest
{
    public const string Permission = "view_dashboard";

    public CompanyGetTripCostPeriodTotalsDto FromRequest(long companyId) => new() { CompanyId = companyId };
}
