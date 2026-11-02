using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Admins;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class ListAdminsRequest : BasePagedRequest
{
    public int? Type { get; set; }
    public int? Status { get; set; }
    public List<long>? RoleIds { get; set; }

    public ListAdminsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Type = Type,
        Status = Status,
        RoleIds = RoleIds
    };
}

public class ListAdminsRequestValidator : AbstractValidator<ListAdminsRequest>
{
    public ListAdminsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleForEach(x => x.RoleIds)
            .GreaterThan(0)
            .WithMessage("validation.must_be_greater_than_zero")
            .When(x => x.RoleIds is { Count: > 0 });
    }
}
