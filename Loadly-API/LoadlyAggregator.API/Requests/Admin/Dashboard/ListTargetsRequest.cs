using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class ListTargetsRequest : BasePagedRequest
{
    public const string Permission = "view_dashboard";

    public int? Year { get; set; }
    public TargetType? TargetType { get; set; }

    public ListTargetsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Year = Year,
        TargetType = TargetType
    };
}

public class ListTargetsRequestValidator : AbstractValidator<ListTargetsRequest>
{
    public ListTargetsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .When(x => x.Year.HasValue)
            .WithMessage("validation.year_between_2000_2100");
        RuleFor(x => x.TargetType)
            .IsValidEnum<ListTargetsRequest, TargetType>("TargetType")
            .When(x => x.TargetType.HasValue);
    }
}
