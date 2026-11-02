using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods.Requests;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;

public class DeleteCommunicationMethodRequest
{
    public const string Permission = "delete_communication_method";

    public long Id { get; set; }

    public DeleteCommunicationMethodDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteCommunicationMethodRequestValidator : AbstractValidator<DeleteCommunicationMethodRequest>
{
    public DeleteCommunicationMethodRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Communication method ID");
    }
}
