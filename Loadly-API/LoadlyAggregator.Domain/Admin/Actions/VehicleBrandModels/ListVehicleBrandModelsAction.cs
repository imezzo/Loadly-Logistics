using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleBrandModels;

public class ListVehicleBrandModelsAction
{
    private readonly IVehicleBrandModelRepository _vehicleBrandModelRepository;
    private readonly IMapper _mapper;

    public ListVehicleBrandModelsAction(IVehicleBrandModelRepository vehicleBrandModelRepository, IMapper mapper)
    {
        _vehicleBrandModelRepository = vehicleBrandModelRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<VehicleBrandModelDto>> ExecuteAsync(ListVehicleBrandModelsDto dto)
    {
        var (vehicleBrandModels, totalCount) = await _vehicleBrandModelRepository.ListAsync(
            dto.Search,
            dto.VehicleBrandId,
            dto.Page,
            dto.PageSize);
        return new ListingDto<VehicleBrandModelDto>
        {
            Data = _mapper.Map<List<VehicleBrandModelDto>>(vehicleBrandModels),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
