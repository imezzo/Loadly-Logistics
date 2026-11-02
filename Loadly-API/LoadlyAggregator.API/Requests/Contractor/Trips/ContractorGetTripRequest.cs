using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorGetTripRequest
{
    public long Id { get; set; }

    public Domain.Contractor.DTOs.Trips.ContractorGetTripDto FromRequest(long contractorId) => new()
    {
        Id = Id,
        ContractorId = contractorId
    };
}

public class ContractorGetTripRequestValidator : AbstractValidator<ContractorGetTripRequest>
{
    public ContractorGetTripRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Trip ID");
    }
}
