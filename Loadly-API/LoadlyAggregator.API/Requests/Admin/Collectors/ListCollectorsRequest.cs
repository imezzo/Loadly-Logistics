using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Collectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class ListCollectorsRequest : BasePagedRequest
{
    public int? Status { get; set; }

    public ListCollectorsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}

public class ListCollectorsRequestValidator : AbstractValidator<ListCollectorsRequest>
{
    public ListCollectorsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
