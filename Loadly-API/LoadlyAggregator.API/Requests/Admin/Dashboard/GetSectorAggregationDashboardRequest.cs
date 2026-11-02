using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class GetSectorAggregationDashboardRequest
{
    public const string Permission = "view_dashboard";

    public DateTime? StartDate { get; set; }

    public GetSectorAggregationDashboardDto FromRequest() => new()
    {
        StartDate = DateTimeHelper.ToUtcNullable(StartDate)
    };
}

public class GetSectorAggregationDashboardRequestValidator : AbstractValidator<GetSectorAggregationDashboardRequest>
{
    public GetSectorAggregationDashboardRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .Must(value => !value.HasValue || value.Value.Year is >= 2000 and <= 2100)
            .WithMessage("validation.start_date_invalid");
    }
}
