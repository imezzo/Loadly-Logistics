using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class GetLeadByPhoneAction
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public GetLeadByPhoneAction(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadDto> ExecuteAsync(GetLeadByPhoneDto dto)
    {
        var lead = await _leadRepository.GetActiveByPhoneAsync(dto.Phone)
            ?? throw new NotFoundException("leads.not_found_with_phone", dto.Phone);
        return _mapper.Map<LeadDto>(lead);
    }
}
