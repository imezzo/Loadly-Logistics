using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public class GetVehicleTypeAction
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IMapper _mapper;

    public GetVehicleTypeAction(IVehicleTypeRepository vehicleTypeRepository, IMapper mapper)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
        _mapper = mapper;
    }

    public async Task<VehicleTypeDto> ExecuteAsync(GetVehicleTypeDto dto)
    {
        var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleTypeDto>(vehicleType);
    }
}
