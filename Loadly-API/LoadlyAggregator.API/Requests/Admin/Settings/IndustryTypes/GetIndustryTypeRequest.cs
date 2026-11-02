using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;

public class GetIndustryTypeRequest
{
    public const string Permission = "view_industry_type";

    public long Id { get; set; }

    public GetIndustryTypeDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetIndustryTypeRequestValidator : AbstractValidator<GetIndustryTypeRequest>
{
    public GetIndustryTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Industry type ID");
    }
}
