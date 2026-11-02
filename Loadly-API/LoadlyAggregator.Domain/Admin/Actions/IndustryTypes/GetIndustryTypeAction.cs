using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;

public class GetIndustryTypeAction
{
    private readonly IIndustryTypeRepository _industryTypeRepository;
    private readonly IMapper _mapper;

    public GetIndustryTypeAction(IIndustryTypeRepository industryTypeRepository, IMapper mapper)
    {
        _industryTypeRepository = industryTypeRepository;
        _mapper = mapper;
    }

    public async Task<IndustryTypeDto> ExecuteAsync(GetIndustryTypeDto dto)
    {
        var industryType = await _industryTypeRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<IndustryTypeDto>(industryType);
    }
}
