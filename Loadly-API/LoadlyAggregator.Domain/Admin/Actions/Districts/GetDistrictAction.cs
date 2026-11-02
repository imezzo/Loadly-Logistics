using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Districts;

public class GetDistrictAction
{
    private readonly IDistrictRepository _districtRepository;
    private readonly IMapper _mapper;

    public GetDistrictAction(IDistrictRepository districtRepository, IMapper mapper)
    {
        _districtRepository = districtRepository;
        _mapper = mapper;
    }

    public async Task<DistrictDto> ExecuteAsync(GetDistrictDto dto)
    {
        var district = await _districtRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<DistrictDto>(district);
    }
}
