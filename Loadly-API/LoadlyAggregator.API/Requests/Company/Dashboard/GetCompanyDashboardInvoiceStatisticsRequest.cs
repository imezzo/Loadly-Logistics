using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Company.Dashboard;

public class GetCompanyDashboardInvoiceStatisticsRequest
{
    public const string Permission = "view_dashboard";

    public CompanyGetInvoiceStatisticsDto FromRequest(long companyId) => new() { CompanyId = companyId };
}
