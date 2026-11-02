using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.OperatingHours;

public class GetOperatingHoursAction
{
    private readonly IOperatingHoursRepository _operatingHoursRepository;
    private readonly IMapper _mapper;

    public GetOperatingHoursAction(
        IOperatingHoursRepository operatingHoursRepository,
        IMapper mapper)
    {
        _operatingHoursRepository = operatingHoursRepository;
        _mapper = mapper;
    }

    public async Task<OperatingHoursDto> ExecuteAsync(GetOperatingHoursDto dto)
    {
        var operatingHours = await _operatingHoursRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<OperatingHoursDto>(operatingHours);
    }
}
