using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class SetContractorPayoutCancelledAction
{
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SetContractorPayoutCancelledAction(
        IContractorPayoutRepository contractorPayoutRepository,
        ITripRepository tripRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _contractorPayoutRepository = contractorPayoutRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(SetContractorPayoutCancelledDto dto)
    {
        var payout = await _contractorPayoutRepository.GetByIdOrThrowAsync(dto.Id);

        if (payout.Status == ContractorPayoutStatus.Cancelled)
            throw new ValidationException("contractor_payouts.payout_already_cancelled");

        if (payout.Status == ContractorPayoutStatus.Paid)
            throw new ValidationException("contractor_payouts.cannot_cancel_paid_payout");

        var linkedTrips = payout.Trips.ToList();

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var trip in linkedTrips)
                trip.ContractorPayoutId = null;

            if (linkedTrips.Count > 0)
                await _tripRepository.UpdateRangeAsync(linkedTrips);

            payout.Status = ContractorPayoutStatus.Cancelled;
            payout.CancelledBy = _currentUserService.GetCurrentUserId();
            payout.CancelledAt = DateTime.UtcNow;
            payout.CancellationReason = dto.CancellationReason;
            _contractorPayoutRepository.Update(payout);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
