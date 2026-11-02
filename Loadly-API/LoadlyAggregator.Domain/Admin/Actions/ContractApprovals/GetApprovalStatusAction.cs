using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;

public class GetApprovalStatusAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly IContractStatusHistoryRepository _historyRepository;
    private readonly IMapper _mapper;

    public GetApprovalStatusAction(
        IContractRepository contractRepository,
        IContractApprovalRepository approvalRepository,
        IContractStatusHistoryRepository historyRepository,
        IMapper mapper)
    {
        _contractRepository = contractRepository;
        _approvalRepository = approvalRepository;
        _historyRepository = historyRepository;
        _mapper = mapper;
    }

    public async Task<object> ExecuteAsync(GetApprovalStatusDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);
        var approvals = await _approvalRepository.GetApprovalsByContractIdAsync(dto.ContractId);
        var history = await _historyRepository.GetHistoryByContractIdAsync(dto.ContractId);
        var latestRejection = await _approvalRepository.GetLatestRejectionByContractIdAsync(dto.ContractId);

        return new
        {
            ContractId = contract.Id,
            ContractName = contract.NameEn,
            CurrentStatus = contract.Status,
            CurrentStatusName = contract.Status.ToString(),
            CurrentApprovalStage = contract.CurrentApprovalStage,
            CurrentApprovalStageName = contract.CurrentApprovalStage is ApprovalStage stage ? stage.ToLocalizedDisplay() : null,
            Approvals = _mapper.Map<List<ContractApprovalDto>>(approvals),
            StatusHistory = _mapper.Map<List<ContractStatusHistoryDto>>(history),
            RejectionReason = latestRejection?.Comments
        };
    }
}
