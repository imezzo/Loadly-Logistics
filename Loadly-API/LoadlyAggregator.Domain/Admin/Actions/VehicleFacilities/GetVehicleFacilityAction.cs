using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;

public class GetVehicleFacilityAction
{
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IMapper _mapper;

    public GetVehicleFacilityAction(IVehicleFacilityRepository vehicleFacilityRepository, IMapper mapper)
    {
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _mapper = mapper;
    }

    public async Task<VehicleFacilityDto> ExecuteAsync(GetVehicleFacilityDto dto)
    {
        var vehicleLocation = await _vehicleFacilityRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleFacilityDto>(vehicleLocation);
    }
}
