using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class GetContractVehicleTypeIdRequest
{
    public const string Permission = GetContractRequest.Permission;

    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }

    public GetContractVehicleTypeByVehicleTypeDto FromRequest() => new()
    {
        ContractId = ContractId,
        VehicleTypeId = VehicleTypeId
    };
}

public class GetContractVehicleTypeIdRequestValidator : AbstractValidator<GetContractVehicleTypeIdRequest>
{
    public GetContractVehicleTypeIdRequestValidator()
    {
        RuleFor(x => x.ContractId).IsRequiredId("Contract ID");
        RuleFor(x => x.VehicleTypeId).IsRequiredId("Vehicle type ID");
    }
}
