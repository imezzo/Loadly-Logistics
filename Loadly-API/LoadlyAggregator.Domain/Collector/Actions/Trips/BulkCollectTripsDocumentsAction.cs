using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Trips;

public class BulkCollectTripsDocumentsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkCollectTripsDocumentsAction(ITripRepository tripRepository, IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(BulkCollectTripsDocumentsDto dto)
    {
        var tripIds = dto.TripIds.Distinct().ToList();

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var tripsById = (await _tripRepository.GetByIdsAsync(tripIds))
                .ToDictionary(t => t.Id);

            foreach (var tripId in tripIds)
            {
                if (!tripsById.TryGetValue(tripId, out var trip))
                {
                    throw new NotFoundException("trips.not_found_with_id", tripId);
                }

                if (trip.Status is not (TripStatus.InProgress or TripStatus.Completed or TripStatus.Scheduled))
                {
                    throw new ValidationException("trips.collect_documents_invalid_status");
                }

                if (!(trip.TripDocumentStatus < TripDocumentStatus.WithFactoryCollector))
                {
                    throw new ValidationException("trips.bulk_collect_invalid_document_status");
                }
            }

            await _tripRepository.BulkUpdateDocumentStatusAsync(tripIds, dto.CollectorId);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
