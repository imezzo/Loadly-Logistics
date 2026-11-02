using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;

public class GetServiceRequirementRequest
{
    public const string Permission = "view_service_requirement";

    public long Id { get; set; }

    public GetServiceRequirementDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetServiceRequirementRequestValidator : AbstractValidator<GetServiceRequirementRequest>
{
    public GetServiceRequirementRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Service requirement ID");
    }
}
