using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class DeleteLeadRequest
{
    public const string Permission = "delete_lead";

    public long Id { get; set; }

    public DeleteLeadDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteLeadRequestValidator : AbstractValidator<DeleteLeadRequest>
{
    public DeleteLeadRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Lead ID");
    }
}
