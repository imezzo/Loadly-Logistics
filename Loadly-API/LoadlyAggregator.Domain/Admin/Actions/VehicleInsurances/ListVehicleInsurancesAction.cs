using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleInsurances;

public class ListVehicleInsurancesAction
{
    private readonly IVehicleInsuranceRepository _vehicleInsuranceRepository;
    private readonly IMapper _mapper;

    public ListVehicleInsurancesAction(
        IVehicleInsuranceRepository vehicleInsuranceRepository,
        IMapper mapper)
    {
        _vehicleInsuranceRepository = vehicleInsuranceRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleInsuranceDto>> ExecuteAsync(ListVehicleInsurancesDto dto)
    {
        var (vehicleInsurances, totalCount) = await _vehicleInsuranceRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<VehicleInsuranceDto>
        {
            Data = _mapper.Map<List<VehicleInsuranceDto>>(vehicleInsurances),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
