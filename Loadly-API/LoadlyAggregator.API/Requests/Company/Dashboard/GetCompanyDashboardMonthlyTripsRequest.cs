using FluentValidation;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.API.Requests.Company.Dashboard;

public class GetCompanyDashboardMonthlyTripsRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }
    public int? Month { get; set; }

    public CompanyGetDashboardMonthlyTripsDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        Year = Year ?? AppUtcTime.UtcCalendarYear(TimeProvider.System),
        Month = Month
    };
}

public class GetCompanyDashboardMonthlyTripsRequestValidator : AbstractValidator<GetCompanyDashboardMonthlyTripsRequest>
{
    public GetCompanyDashboardMonthlyTripsRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .When(x => x.Year.HasValue)
            .WithMessage("Year must be between 2000 and 2100");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .When(x => x.Month.HasValue)
            .WithMessage("Month must be between 1 and 12");
    }
}
