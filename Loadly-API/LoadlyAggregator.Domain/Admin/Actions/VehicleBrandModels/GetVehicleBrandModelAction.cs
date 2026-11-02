using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;

public class GetVehicleBrandModelAction
{
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IMapper _mapper;

    public GetVehicleBrandModelAction(IVehicleBrandModelRepository vehicleBrandModelRepository, IMapper mapper)
    {
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _mapper = mapper;
    }

    public async Task<VehicleBrandModelDto> ExecuteAsync(GetVehicleBrandModelDto dto)
    {
        var vehicleBrandModel = await _vehicleBrandModelRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleBrandModelDto>(vehicleBrandModel);
    }
}
