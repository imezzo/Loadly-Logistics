using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;

public class ListCommunicationMethodsRequest : BasePagedRequest
{
    public const string Permission = "view_communication_method";


    public ListCommunicationMethodsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListCommunicationMethodsRequestValidator : AbstractValidator<ListCommunicationMethodsRequest>
{
    public ListCommunicationMethodsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
