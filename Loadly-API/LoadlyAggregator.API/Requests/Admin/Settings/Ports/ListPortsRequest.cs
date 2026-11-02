using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Ports;

public class ListPortsRequest : BasePagedRequest
{
    public const string Permission = "view_port";

    public PortType? Type { get; set; }
    public PortStatus? Status { get; set; }

    public ListPortsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Type = Type,
        Status = Status
    };
}

public class ListPortsRequestValidator : AbstractValidator<ListPortsRequest>
{
    public ListPortsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Type)
            .IsValidEnum<ListPortsRequest, PortType>("Type")
            .When(x => x.Type.HasValue);
        RuleFor(x => x.Status)
            .IsValidEnum<ListPortsRequest, PortStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
