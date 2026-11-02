using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Taxes;

public class GetTaxRequest
{
    public const string Permission = "view_tax";

    public long Id { get; set; }

    public GetTaxDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetTaxRequestValidator : AbstractValidator<GetTaxRequest>
{
    public GetTaxRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Tax ID");
    }
}
