using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class ListLeadsRequest : BasePagedRequest
{
    public const string Permission = "view_lead";

    public ListLeadsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListLeadsRequestValidator : AbstractValidator<ListLeadsRequest>
{
    public ListLeadsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
