using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;

public class GetCommunicationMethodRequest
{
    public const string Permission = "view_communication_method";

    public long Id { get; set; }

    public GetCommunicationMethodDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetCommunicationMethodRequestValidator : AbstractValidator<GetCommunicationMethodRequest>
{
    public GetCommunicationMethodRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Communication method ID");
    }
}
