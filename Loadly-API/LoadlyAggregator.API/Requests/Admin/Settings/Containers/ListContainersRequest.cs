using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Containers;

public class ListContainersRequest : BasePagedRequest
{
    public const string Permission = "view_container";

    public ContainerStatus? Status { get; set; }

    public ListContainersDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}

public class ListContainersRequestValidator : AbstractValidator<ListContainersRequest>
{
    public ListContainersRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Status)
            .IsValidEnum<ListContainersRequest, ContainerStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
