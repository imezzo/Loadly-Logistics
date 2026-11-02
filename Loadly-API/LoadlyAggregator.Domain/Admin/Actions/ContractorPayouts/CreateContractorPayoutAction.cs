using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;

public class CreateContractorPayoutAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IContractorPayoutRepository _contractorPayoutRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateContractorPayoutAction(
        ITripRepository tripRepository,
        IContractorPayoutRepository contractorPayoutRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _contractorPayoutRepository = contractorPayoutRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<long> ExecuteAsync(CreateContractorPayoutDto dto)
    {
        var tripIds = dto.TripIds.Distinct().ToList();
        var fetchedTrips = (await _tripRepository.GetByIdsAsync(tripIds)).ToDictionary(t => t.Id);
        var trips = ContractorPayoutTripSelection.ResolveEligibleTrips(tripIds, fetchedTrips);

        var totalAmount = trips.Sum(t => t.ContractorPrice!.Value);
        if (totalAmount <= 0)
            throw new LogicalException("contractor_payouts.no_eligible_trips");

        var contractorId = trips[0].ContractorId!.Value;
        var addedBy = _currentUserService.GetCurrentUserId();

        var year = DateTime.UtcNow.Year;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var nextSequence = await _contractorPayoutRepository.AllocateNextPayoutSequenceForYearAsync(year);
            var payout = new ContractorPayout
            {
                PayoutNumber = $"CPY-{year}-{nextSequence:D6}",
                Sequence = nextSequence,
                ContractorId = contractorId,
                TotalAmount = totalAmount,
                Status = ContractorPayoutStatus.Pending,
                AddedBy = addedBy
            };
            await _contractorPayoutRepository.AddAsync(payout);

            foreach (var trip in trips)
                trip.ContractorPayout = payout;

            await _tripRepository.UpdateRangeAsync(trips);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return payout.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
