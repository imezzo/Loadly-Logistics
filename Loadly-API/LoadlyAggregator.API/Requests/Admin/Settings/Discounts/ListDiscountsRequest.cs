using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.Discounts.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Discounts;

public class ListDiscountsRequest : BasePagedRequest
{
    public const string Permission = "view_discount";

    public DiscountType? Type { get; set; }
    public DiscountStatus? Status { get; set; }

    public ListDiscountsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Type = Type,
        Status = Status
    };
}

public class ListDiscountsRequestValidator : AbstractValidator<ListDiscountsRequest>
{
    public ListDiscountsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Type)
            .IsValidEnum<ListDiscountsRequest, Domain.Common.Enums.DiscountType>("Type")
            .When(x => x.Type.HasValue);
        RuleFor(x => x.Status)
            .IsValidEnum<ListDiscountsRequest, Domain.Common.Enums.DiscountStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
