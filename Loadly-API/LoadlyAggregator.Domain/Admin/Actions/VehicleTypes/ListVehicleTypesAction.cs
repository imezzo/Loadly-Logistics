using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public class ListVehicleTypesAction
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IMapper _mapper;

    public ListVehicleTypesAction(IVehicleTypeRepository vehicleTypeRepository, IMapper mapper)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleTypeDto>> ExecuteAsync(ListVehicleTypesDto dto)
    {
        var (vehicleTypes, totalCount) = await _vehicleTypeRepository.ListAsync(
            dto.Search,
            dto.Category,
            dto.VehicleUnitTypes,
            dto.Status,
            dto.Page,
            dto.PageSize);
        return new ListingDto<VehicleTypeDto>
        {
            Data = _mapper.Map<List<VehicleTypeDto>>(vehicleTypes),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
