using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class GetLeadAction
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public GetLeadAction(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadDto> ExecuteAsync(GetLeadDto dto)
    {
        var lead = await _leadRepository.GetByIdOrThrowAsync(dto.Id);
        var counts = await _leadRepository.GetAggregateCountsAsync(dto.Id);

        var leadDto = _mapper.Map<LeadDto>(lead);
        leadDto.TotalCompletedTrips = counts.TotalCompletedTrips;

        return leadDto;
    }
}
