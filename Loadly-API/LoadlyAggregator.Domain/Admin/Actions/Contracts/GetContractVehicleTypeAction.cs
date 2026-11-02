using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractVehicleTypeAction
{
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly IMapper _mapper;

    public GetContractVehicleTypeAction(
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        IMapper mapper)
    {
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _mapper = mapper;
    }

    public async Task<ContractVehicleTypeDto> ExecuteAsync(GetContractVehicleTypeDto dto)
    {
        var contractVehicleType = await _contractVehicleTypeRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ContractVehicleTypeDto>(contractVehicleType);
    }
}
