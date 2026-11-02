using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Trips;

public class BulkConfirmTripsDocumentsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkConfirmTripsDocumentsAction(ITripRepository tripRepository, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(BulkConfirmTripsDocumentsDto dto)
    {
        var tripIds = dto.TripIds.Distinct().ToList();

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var tripsById = (await _tripRepository.GetByIdsAsync(tripIds))
                .ToDictionary(t => t.Id);

            foreach (var tripId in tripIds)
            {
                if (!tripsById.TryGetValue(tripId, out _))
                {
                    throw new NotFoundException("trips.not_found_with_id", tripId);
                }
            }

            await _tripRepository.BulkUpdateDocumentStatusToFinanceTeamAsync(tripIds, dto.FinanceCollectorId);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
