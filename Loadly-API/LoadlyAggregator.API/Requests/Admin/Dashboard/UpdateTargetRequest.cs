using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class UpdateTargetRequest
{
    public const string Permission = "manage_dashboard_targets";

    [FromRoute]
    public long Id { get; set; }
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }

    public UpdateTargetDto FromRequest() => new()
    {
        Id = Id,
        Month = Month,
        Year = Year,
        TargetType = TargetType,
        TargetValue = TargetValue
    };
}

public class UpdateTargetRequestValidator : AbstractValidator<UpdateTargetRequest>
{
    public UpdateTargetRequestValidator(ITargetRepository targetRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Target ID");
        RuleFor(x => x.Month)
            .IsValidEnum<UpdateTargetRequest, TargetMonth>("Month");
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage("validation.year_between_2000_2100");
        RuleFor(x => x.TargetType)
            .IsValidEnum<UpdateTargetRequest, TargetType>("TargetType");
        RuleFor(x => x.TargetValue)
            .IsGreaterThanZero("TargetValue");
        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
            {
                var existing = await targetRepository.GetByMonthYearAndTypeAsync(req.Month, req.Year, req.TargetType, req.Id);
                return existing == null;
            })
            .WithMessage("validation.target_duplicate_month_year");
    }
}
