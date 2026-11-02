using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Discounts;

public class DeleteDiscountRequest
{
    public const string Permission = "delete_discount";

    public long Id { get; set; }

    public DeleteDiscountDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteDiscountRequestValidator : AbstractValidator<DeleteDiscountRequest>
{
    public DeleteDiscountRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Discount ID");
    }
}
