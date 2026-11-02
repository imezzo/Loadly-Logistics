using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Taxes;

public class DeleteTaxRequest
{
    public const string Permission = "delete_tax";

    public long Id { get; set; }

    public DeleteTaxDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteTaxRequestValidator : AbstractValidator<DeleteTaxRequest>
{
    public DeleteTaxRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Tax ID");
    }
}
