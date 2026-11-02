using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class GetContractVehicleTypeRequest
{
    public const string Permission = GetContractRequest.Permission;

    public long Id { get; set; }

    public GetContractVehicleTypeDto FromRequest() => new() { Id = Id };
}

public class GetContractVehicleTypeRequestValidator : AbstractValidator<GetContractVehicleTypeRequest>
{
    public GetContractVehicleTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Contract vehicle type ID");
    }
}
