using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Company.Dashboard;

public class GetCompanyDashboardHomeRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }
    public bool LastMonth { get; set; }
    public bool LastWeek { get; set; }

    public CompanyGetHomeDashboardDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        Year = Year,
        LastMonth = LastMonth,
        LastWeek = LastWeek
    };
}

public class GetCompanyDashboardHomeRequestValidator : AbstractValidator<GetCompanyDashboardHomeRequest>
{
    public GetCompanyDashboardHomeRequestValidator()
    {
        RuleFor(x => x)
            .Must(HasAtMostOneDateFilter)
            .WithMessage("validation.dashboard_trips_filters_mutually_exclusive");

        RuleFor(x => x.Year)
            .IsInclusiveRange(2000, 2100, "Year")
            .When(x => x.Year.HasValue);
    }

    private static bool HasAtMostOneDateFilter(GetCompanyDashboardHomeRequest request)
    {
        int activeFilters = 0;
        if (request.LastWeek)
        {
            activeFilters++;
        }

        if (request.LastMonth)
        {
            activeFilters++;
        }

        if (request.Year.HasValue)
        {
            activeFilters++;
        }

        return activeFilters <= 1;
    }
}
