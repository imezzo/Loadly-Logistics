using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;

public class GetVehicleCapacityUnitAction
{
    private readonly IVehicleCapacityUnitRepository _vehicleCapacityUnitRepository;
    private readonly IMapper _mapper;

    public GetVehicleCapacityUnitAction(
        IVehicleCapacityUnitRepository vehicleCapacityUnitRepository,
        IMapper mapper)
    {
        _vehicleCapacityUnitRepository = vehicleCapacityUnitRepository;
        _mapper = mapper;
    }

    public async Task<VehicleCapacityUnitDto> ExecuteAsync(GetVehicleCapacityUnitDto dto)
    {
        var vehicleCapacityUnit = await _vehicleCapacityUnitRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleCapacityUnitDto>(vehicleCapacityUnit);
    }
}
