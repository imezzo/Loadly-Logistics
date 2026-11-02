using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleCapacityUnits;

public class ListVehicleCapacityUnitsAction
{
    private readonly IVehicleCapacityUnitRepository _vehicleCapacityUnitRepository;
    private readonly IMapper _mapper;

    public ListVehicleCapacityUnitsAction(
        IVehicleCapacityUnitRepository vehicleCapacityUnitRepository,
        IMapper mapper)
    {
        _vehicleCapacityUnitRepository = vehicleCapacityUnitRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleCapacityUnitDto>> ExecuteAsync(BasePagedRequest dto)
    {
        var (vehicleCapacityUnits, totalCount) = await _vehicleCapacityUnitRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<VehicleCapacityUnitDto>
        {
            Data = _mapper.Map<List<VehicleCapacityUnitDto>>(vehicleCapacityUnits),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
