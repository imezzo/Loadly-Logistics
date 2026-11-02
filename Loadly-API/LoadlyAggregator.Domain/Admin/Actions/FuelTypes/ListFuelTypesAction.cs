using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.FuelTypes;

public class ListFuelTypesAction
{
    private readonly IFuelTypeRepository _fuelTypeRepository;
    private readonly IMapper _mapper;

    public ListFuelTypesAction(IFuelTypeRepository fuelTypeRepository, IMapper mapper)
    {
        _fuelTypeRepository = fuelTypeRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<FuelTypeDto>> ExecuteAsync(ListFuelTypesDto dto)
    {
        var (fuelTypes, totalCount) = await _fuelTypeRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);
        return new ListingDto<FuelTypeDto>
        {
            Data = _mapper.Map<List<FuelTypeDto>>(fuelTypes),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
