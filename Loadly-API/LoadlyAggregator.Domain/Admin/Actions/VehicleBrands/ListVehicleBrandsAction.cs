using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrands;

public class ListVehicleBrandsAction
{
    private readonly IVehicleBrandRepository _vehicleBrandRepository;
    private readonly IMapper _mapper;

    public ListVehicleBrandsAction(IVehicleBrandRepository vehicleBrandRepository, IMapper mapper)
    {
        _vehicleBrandRepository = vehicleBrandRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleBrandDto>> ExecuteAsync(ListVehicleBrandsDto dto)
    {
        var (vehicleBrands, totalCount) = await _vehicleBrandRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<VehicleBrandDto>
        {
            Data = _mapper.Map<List<VehicleBrandDto>>(vehicleBrands),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
