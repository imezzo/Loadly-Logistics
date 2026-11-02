using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class GetContractorPayoutAction
{
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly IMapper _mapper;

    public GetContractorPayoutAction(
        IContractorPayoutRepository contractorPayoutRepository,
        IMapper mapper)
    {
        _contractorPayoutRepository = contractorPayoutRepository;
        _mapper = mapper;
    }

    public async Task<ContractorPayoutDto> ExecuteAsync(GetContractorPayoutDto dto)
    {
        var payout = await _contractorPayoutRepository.GetByIdOrThrowAsync(dto.Id);
        return _mapper.Map<ContractorPayoutDto>(payout);
    }
}
