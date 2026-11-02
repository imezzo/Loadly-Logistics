using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;

public class GetVehicleBrandAction
{
    private readonly IVehicleBrandRepository _vehicleBrandRepository;
    private readonly IMapper _mapper;

    public GetVehicleBrandAction(IVehicleBrandRepository vehicleBrandRepository, IMapper mapper)
    {
        _vehicleBrandRepository = vehicleBrandRepository;
        _mapper = mapper;
    }

    public async Task<VehicleBrandDto> ExecuteAsync(GetVehicleBrandDto dto)
    {
        var vehicleBrand = await _vehicleBrandRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleBrandDto>(vehicleBrand);
    }
}
