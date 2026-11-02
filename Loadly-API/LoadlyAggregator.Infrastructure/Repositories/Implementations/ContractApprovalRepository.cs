using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractApprovalRepository : Repository<ContractApproval>, IContractApprovalRepository
{
    public ContractApprovalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override Task<ContractApproval?> GetByIdAsync(long id) =>
        _dbSet
            .Include(ca => ca.Contract)
            .Include(ca => ca.Approver)
            .FirstOrDefaultAsync(ca => ca.Id == id);

    public Task<List<ContractApproval>> GetApprovalsByContractIdAsync(long contractId) =>
        _dbSet
            .Include(ca => ca.Approver)
            .Where(ca => ca.ContractId == contractId)
            .OrderBy(ca => ca.StageOrder)
            .ToListAsync();

    public Task<ContractApproval?> GetApprovalByContractAndStageAsync(long contractId, ApprovalStage stage) =>
        _dbSet
            .Include(ca => ca.Approver)
            .FirstOrDefaultAsync(ca => ca.ContractId == contractId && ca.Stage == stage);

    public Task<ContractApproval?> GetCurrentApprovalAsync(long contractId) =>
        _dbSet
            .Include(ca => ca.Approver)
            .Where(ca => ca.ContractId == contractId && ca.Status == ApprovalStatus.PENDING)
            .OrderBy(ca => ca.StageOrder)
            .FirstOrDefaultAsync();

    public Task<ContractApproval?> GetNextPendingApprovalAsync(long contractId, int afterStageOrder) =>
        _dbSet
            .Include(ca => ca.Approver)
            .Where(ca => ca.ContractId == contractId && ca.StageOrder > afterStageOrder && ca.Status == ApprovalStatus.PENDING)
            .OrderBy(ca => ca.StageOrder)
            .FirstOrDefaultAsync();

    public Task<bool> HasUserAlreadyApprovedAtOtherStageAsync(long contractId, long userId, long excludeApprovalId) =>
        _dbSet
            .AnyAsync(ca => ca.ContractId == contractId && ca.ApproverId == userId && ca.Status == ApprovalStatus.APPROVED && ca.Id != excludeApprovalId);

    public Task<List<ContractApproval>> GetPendingApprovalsByContractIdAsync(long contractId) =>
        _dbSet
            .Include(ca => ca.Approver)
            .Where(ca => ca.ContractId == contractId && ca.Status == ApprovalStatus.PENDING)
            .OrderBy(ca => ca.StageOrder)
            .ToListAsync();

    public Task<List<ContractApproval>> GetOtherPendingApprovalsAsync(long contractId, long excludeApprovalId) =>
        _dbSet
            .Include(ca => ca.Approver)
            .Where(ca => ca.ContractId == contractId && ca.Status == ApprovalStatus.PENDING && ca.Id != excludeApprovalId)
            .OrderBy(ca => ca.StageOrder)
            .ToListAsync();

    public Task<ContractApproval?> GetLatestRejectionByContractIdAsync(long contractId) =>
        _dbSet
            .Where(ca => ca.ContractId == contractId && ca.Status == ApprovalStatus.REJECTED && ca.ActionDate != null)
            .OrderByDescending(ca => ca.ActionDate)
            .FirstOrDefaultAsync();

    public Task<List<ContractApproval>> GetPendingApprovalsByApproverIdAsync(long approverId) =>
        _dbSet
            .Include(ca => ca.Contract)
                .ThenInclude(c => c!.Company)
            .Where(ca => ca.ApproverId == approverId && ca.Status == ApprovalStatus.PENDING)
            .OrderBy(ca => ca.CreatedAt)
            .ToListAsync();

    public Task<bool> HasApprovalsForContractAsync(long contractId) =>
        _dbSet.AnyAsync(ca => ca.ContractId == contractId);

    public Task<bool> HasPendingApprovalsAsync(long contractId) =>
        _dbSet
            .AnyAsync(ca => ca.ContractId == contractId && ca.Status == ApprovalStatus.PENDING);

    public async Task<(List<ContractApproval> ContractApprovals, int TotalCount)> ListAsync(
        long? contractId,
        ApprovalStage? stage,
        long? approverId,
        ApprovalStatus? status,
        int page,
        int pageSize) =>
        (await _dbSet
            .AsNoTracking()
            .Include(ca => ca.Contract)
            .Include(ca => ca.Approver)
            .Where(ca => !contractId.HasValue || ca.ContractId == contractId.Value)
            .Where(ca => !stage.HasValue || ca.Stage == stage.Value)
            .Where(ca => !approverId.HasValue || ca.ApproverId == approverId.Value)
            .Where(ca => !status.HasValue || ca.Status == status.Value)
            .OrderByDescending(ca => ca.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(),
        await _dbSet
            .Where(ca => !contractId.HasValue || ca.ContractId == contractId.Value)
            .Where(ca => !stage.HasValue || ca.Stage == stage.Value)
            .Where(ca => !approverId.HasValue || ca.ApproverId == approverId.Value)
            .Where(ca => !status.HasValue || ca.Status == status.Value)
            .CountAsync());

    public async Task<ContractApproval> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contract_approvals.not_found_with_id", id);
}
