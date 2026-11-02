using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;

namespace LoadlyAggregator.API.Requests.Contractor.Drivers;

public class ContractorGetDriverRequest
{
    public long Id { get; set; }

    public Domain.Contractor.DTOs.Drivers.ContractorGetDriverDto FromRequest(long contractorId) => new()
    {
        Id = Id,
        ContractorId = contractorId
    };
}

public class ContractorGetDriverRequestValidator : AbstractValidator<ContractorGetDriverRequest>
{
    public ContractorGetDriverRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Driver ID");
    }
}
