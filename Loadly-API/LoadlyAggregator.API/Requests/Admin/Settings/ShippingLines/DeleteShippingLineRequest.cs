using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;

public class DeleteShippingLineRequest
{
    public const string Permission = "delete_shipping_line";

    public long Id { get; set; }

    public DeleteShippingLineDto FromRequest() => new() { Id = Id };
}

public class DeleteShippingLineRequestValidator : AbstractValidator<DeleteShippingLineRequest>
{
    public DeleteShippingLineRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Shipping line ID");
    }
}
