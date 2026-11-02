using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ShippingLines;

public class GetShippingLineRequest
{
    public const string Permission = "view_shipping_line";

    public long Id { get; set; }

    public GetShippingLineDto FromRequest() => new() { Id = Id };
}

public class GetShippingLineRequestValidator : AbstractValidator<GetShippingLineRequest>
{
    public GetShippingLineRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Shipping line ID");
    }
}
