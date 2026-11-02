using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;

public class DeleteIndustryTypeRequest
{
    public const string Permission = "delete_industry_type";

    public long Id { get; set; }

    public DeleteIndustryTypeDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteIndustryTypeRequestValidator : AbstractValidator<DeleteIndustryTypeRequest>
{
    public DeleteIndustryTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Industry type ID");
    }
}
