using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractStatusHistoryRepository : Repository<ContractStatusHistory>, IContractStatusHistoryRepository
{
    private readonly TimeProvider _timeProvider;

    public ContractStatusHistoryRepository(ApplicationDbContext context, TimeProvider timeProvider) : base(context)
    {
        _timeProvider = timeProvider;
    }

    public override Task<ContractStatusHistory?> GetByIdAsync(long id) =>
        _dbSet
            .Include(csh => csh.Contract)
            .Include(csh => csh.ChangedByAdmin)
            .FirstOrDefaultAsync(csh => csh.Id == id);

    public Task<List<ContractStatusHistory>> GetHistoryByContractIdAsync(long contractId) =>
        _dbSet
            .Include(csh => csh.ChangedByAdmin)
            .Where(csh => csh.ContractId == contractId)
            .OrderByDescending(csh => csh.ChangedAt)
            .ToListAsync();

    public async Task AddHistoryAsync(long contractId, ContractStatus previousStatus, ContractStatus newStatus, long? changedBy, string? additionalNotes = null, ApprovalStage? approvalStage = null)
    {
        await AddAsync(new ContractStatusHistory
        {
            ContractId = contractId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ApprovalStage = approvalStage,
            ChangedBy = changedBy,
            ChangedAt = AppUtcTime.UtcNow(_timeProvider),
            AdditionalNotes = additionalNotes
        });
    }
}
