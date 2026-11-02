using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class ListContractorPayoutsAction
{
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly IMapper _mapper;

    public ListContractorPayoutsAction(
        IContractorPayoutRepository contractorPayoutRepository,
        IMapper mapper)
    {
        _contractorPayoutRepository = contractorPayoutRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ContractorPayoutDto>> ExecuteAsync(ListContractorPayoutsDto dto)
    {
        var (payouts, totalCount) = await _contractorPayoutRepository.ListAsync(
            dto.ContractorId,
            dto.Status,
            dto.Page,
            dto.PageSize);

        return new ListingDto<ContractorPayoutDto>
        {
            Data = _mapper.Map<List<ContractorPayoutDto>>(payouts),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
