using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;

public class DeleteServiceRequirementRequest
{
    public const string Permission = "delete_service_requirement";

    public long Id { get; set; }

    public DeleteServiceRequirementDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteServiceRequirementRequestValidator : AbstractValidator<DeleteServiceRequirementRequest>
{
    public DeleteServiceRequirementRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Service requirement ID");
    }
}
