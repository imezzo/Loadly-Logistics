using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetDashboardTripsRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }

    public bool LastMonth { get; set; }
    public bool LastWeek { get; set; }

    public GetDashboardTripsStatisticsDto FromRequest() => new()
    {
        Year = Year,
        LastMonth = LastMonth,
        LastWeek = LastWeek
    };
}

public class GetDashboardTripsRequestValidator : AbstractValidator<GetDashboardTripsRequest>
{
    public GetDashboardTripsRequestValidator()
    {
        RuleFor(x => x)
            .Must(HasAtMostOneDateFilter)
            .WithMessage("validation.dashboard_trips_filters_mutually_exclusive");

        RuleFor(x => x.Year)
            .IsInclusiveRange(2000, 2100, "Year")
            .When(x => x.Year.HasValue);
    }

    private static bool HasAtMostOneDateFilter(GetDashboardTripsRequest request)
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
