using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;

namespace LoadlyAggregator.API.Requests.Contractor.Vehicles;

public class ContractorGetVehicleRequest
{
    public long Id { get; set; }

    public Domain.Contractor.DTOs.Vehicles.ContractorGetVehicleDto FromRequest(long contractorId) => new()
    {
        Id = Id,
        ContractorId = contractorId
    };
}

public class ContractorGetVehicleRequestValidator : AbstractValidator<ContractorGetVehicleRequest>
{
    public ContractorGetVehicleRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle ID");
    }
}
