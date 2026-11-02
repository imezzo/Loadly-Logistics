using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;

public class SubmitContractApprovalDecisionAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractApprovalPipeline _approvalPipeline;
    private readonly IContractApprovalRepository _approvalRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    private readonly ApproveContractApprovalAction _approveAction;
    private readonly RejectContractApprovalAction _rejectAction;

    public SubmitContractApprovalDecisionAction(
        IContractRepository contractRepository,
        IContractApprovalPipeline approvalPipeline,
        IContractApprovalRepository approvalRepository,
        ICurrentUserService currentUserService,
        IAdminRepository adminRepository,
        IPermissionService permissionService,
        ApproveContractApprovalAction approveAction,
        RejectContractApprovalAction rejectAction)
    {
        _contractRepository = contractRepository;
        _approvalPipeline = approvalPipeline;
        _approvalRepository = approvalRepository;
        _currentUserService = currentUserService;
        _adminRepository = adminRepository;
        _permissionService = permissionService;
        _approveAction = approveAction;
        _rejectAction = rejectAction;
    }

    public async Task ExecuteAsync(ContractApprovalDecisionDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);
        EnsureContractPendingApproval(contract);

        var currentApproval = await _approvalRepository.GetCurrentApprovalAsync(dto.ContractId)
            ?? throw new ValidationException("contracts.no_pending_approval");

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        await EnsureCurrentUserCanActAtStageAsync(currentUserId, currentApproval);

        await (dto.Action == ContractApprovalDecision.Approve
            ? _approveAction.ExecuteAsync(contract, currentApproval, currentUserId, dto)
            : _rejectAction.ExecuteAsync(contract, currentApproval, currentUserId, dto));
    }

    private static void EnsureContractPendingApproval(Contract contract)
    {
        if (contract.Status != ContractStatus.PENDING_APPROVAL)
            throw new ValidationException("contracts.only_pending_approval_can_be_decided");
    }

    private async Task EnsureCurrentUserCanActAtStageAsync(long currentUserId, ContractApproval currentApproval)
    {
        var admin = await _adminRepository.GetByIdOrThrowAsync(currentUserId);
        if (admin.Status != UserStatus.ACTIVE)
            throw new UnauthorizedException("auth.account_not_active");

        var stageName = currentApproval.Stage.ToLocalizedDisplay();
        var requiredPermission = _approvalPipeline.GetRequiredPermission(currentApproval.Stage);
        if (string.IsNullOrEmpty(requiredPermission))
            throw new ForbiddenException("contract_approvals.no_permission_configured_for_stage", stageName);

        // Retry after clearing permission cache (handles stale cache after role/permission assignment)
        if (!await _permissionService.HasPermissionToAsync(admin, requiredPermission))
        {
            _permissionService.ClearUserCache(admin);
            if (!await _permissionService.HasPermissionToAsync(admin, requiredPermission))
                throw new ForbiddenException("contract_approvals.waiting_for_stage_approval", stageName);
        }
    }
}
