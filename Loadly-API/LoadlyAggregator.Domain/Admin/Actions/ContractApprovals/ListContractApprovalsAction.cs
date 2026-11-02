using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;

public class ListContractApprovalsAction
{
    private readonly IContractApprovalRepository _contractApprovalRepository;
    private readonly IMapper _mapper;

    public ListContractApprovalsAction(
        IContractApprovalRepository contractApprovalRepository,
        IMapper mapper)
    {
        _contractApprovalRepository = contractApprovalRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<ContractApprovalDto>> ExecuteAsync(ListContractApprovalsDto dto)
    {
        var (contractApprovals, totalCount) = await _contractApprovalRepository.ListAsync(
            dto.ContractId,
            dto.Stage,
            dto.ApproverId,
            dto.Status,
            dto.Page,
            dto.PageSize);

        return new ListingDto<ContractApprovalDto>
        {
            Data = _mapper.Map<List<ContractApprovalDto>>(contractApprovals),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
