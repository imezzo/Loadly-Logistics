using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;

public class ListVehicleFacilitiesAction
{
    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IMapper _mapper;

    public ListVehicleFacilitiesAction(IVehicleFacilityRepository vehicleFacilityRepository, IMapper mapper)
    {
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleFacilityDto>> ExecuteAsync(ListVehicleFacilitiesDto dto)
    {
        var (vehicleLocations, totalCount) = await _vehicleFacilityRepository.ListAsync(
            dto.Search,
            dto.Status,
            dto.FacilityType,
            dto.CompanyIds,
            dto.Page,
            dto.PageSize);

        return new ListingDto<VehicleFacilityDto>
        {
            Data = _mapper.Map<List<VehicleFacilityDto>>(vehicleLocations),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
