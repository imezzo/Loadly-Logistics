using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.FuelTypes;

public class GetFuelTypeAction
{
    private readonly IFuelTypeRepository _fuelTypeRepository;
    private readonly IMapper _mapper;

    public GetFuelTypeAction(IFuelTypeRepository fuelTypeRepository, IMapper mapper)
    {
        _fuelTypeRepository = fuelTypeRepository;
        _mapper = mapper;
    }

    public async Task<FuelTypeDto> ExecuteAsync(GetFuelTypeDto dto)
    {
        var fuelType = await _fuelTypeRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<FuelTypeDto>(fuelType);
    }
}
