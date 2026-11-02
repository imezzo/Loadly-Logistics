using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Discounts;

public class GetDiscountRequest
{
    public const string Permission = "view_discount";

    public long Id { get; set; }

    public GetDiscountDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetDiscountRequestValidator : AbstractValidator<GetDiscountRequest>
{
    public GetDiscountRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Discount ID");
    }
}
