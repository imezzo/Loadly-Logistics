using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class CreateTargetRequest
{
    public const string Permission = "manage_dashboard_targets";

    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }

    public CreateTargetDto FromRequest() => new()
    {
        Month = Month,
        Year = Year,
        TargetType = TargetType,
        TargetValue = TargetValue
    };
}

public class CreateTargetRequestValidator : AbstractValidator<CreateTargetRequest>
{
    public CreateTargetRequestValidator(ITargetRepository targetRepository)
    {
        RuleFor(x => x.Month)
            .IsValidEnum<CreateTargetRequest, TargetMonth>("Month");
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage("validation.year_between_2000_2100");
        RuleFor(x => x.TargetType)
            .IsValidEnum<CreateTargetRequest, TargetType>("TargetType");
        RuleFor(x => x.TargetValue)
            .IsGreaterThanZero("TargetValue");
        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
                await targetRepository.GetByMonthYearAndTypeAsync(req.Month, req.Year, req.TargetType) == null)
            .WithMessage("validation.target_duplicate_month_year");
    }
}
