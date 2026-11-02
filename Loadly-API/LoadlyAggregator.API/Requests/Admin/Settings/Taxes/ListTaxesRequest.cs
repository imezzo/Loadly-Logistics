using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Taxes;

public class ListTaxesRequest : BasePagedRequest
{
    public const string Permission = "view_tax";

    public TaxType? Type { get; set; }
    public TaxStatus? Status { get; set; }

    public ListTaxesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Type = Type,
        Status = Status
    };
}

public class ListTaxesRequestValidator : AbstractValidator<ListTaxesRequest>
{
    public ListTaxesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Type)
            .IsValidEnum<ListTaxesRequest, Domain.Common.Enums.TaxType>("Type")
            .When(x => x.Type.HasValue);
        RuleFor(x => x.Status)
            .IsValidEnum<ListTaxesRequest, Domain.Common.Enums.TaxStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
