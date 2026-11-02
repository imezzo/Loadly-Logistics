using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;

public class GetVehicleInsuranceAction
{
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;
    private readonly IMapper _mapper;

    public GetVehicleInsuranceAction(
        IVehicleInsuranceRepository vehicleInsuranceRepository,
        IMapper mapper)
    {
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
        _mapper = mapper;
    }

    public async Task<VehicleInsuranceDto> ExecuteAsync(GetVehicleInsuranceDto dto)
    {
        var vehicleInsurance = await _vehicleInsuranceRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<VehicleInsuranceDto>(vehicleInsurance);
    }
}
