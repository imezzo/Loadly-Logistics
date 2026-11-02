using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractVehicleTypeIdAction
{
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;

    public GetContractVehicleTypeIdAction(IContractVehicleTypeRepository contractVehicleTypeRepository)
    {
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
    }

    public async Task<ContractVehicleTypeIdDto> ExecuteAsync(GetContractVehicleTypeByVehicleTypeDto dto)
    {
        var contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(
            dto.ContractId,
            dto.VehicleTypeId);

        return contractVehicleType is null
            ? throw new NotFoundException(
                "contract_vehicle_types.not_found_for_contract_and_vehicle_type",
                dto.ContractId,
                dto.VehicleTypeId)
            : new ContractVehicleTypeIdDto { Id = contractVehicleType.Id };
    }
}
