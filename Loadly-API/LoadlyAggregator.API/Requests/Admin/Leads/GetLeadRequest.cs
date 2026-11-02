using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class GetLeadRequest
{
    public const string Permission = "view_lead";

    public long Id { get; set; }

    public GetLeadDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetLeadRequestValidator : AbstractValidator<GetLeadRequest>
{
    public GetLeadRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Lead ID");
    }
}
