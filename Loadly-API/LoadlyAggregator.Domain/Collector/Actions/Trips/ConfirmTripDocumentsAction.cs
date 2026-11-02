using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Trips;

public class ConfirmTripDocumentsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITripDocumentRepository _tripDocumentRepository;
    private readonly IMediaService _mediaService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmTripDocumentsAction(
        ITripRepository tripRepository,
        IDocumentRepository documentRepository,
        ITripDocumentRepository tripDocumentRepository,
        IMediaService mediaService,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _documentRepository = documentRepository;
        _tripDocumentRepository = tripDocumentRepository;
        _mediaService = mediaService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ConfirmTripDocumentsDto dto)
    {
        var newlyUploadedPaths = new List<string>();
        var pathsToCleanupAfterCommit = new List<string>();

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
            if (trip.Status is not (TripStatus.InProgress or TripStatus.Completed or TripStatus.Scheduled))
            {
                throw new ValidationException("trips.collect_documents_invalid_status");
            }

            trip.FinanceCollectorId = dto.FinanceCollectorId;
            trip.TripDocumentStatus = dto.Submit
                ? TripDocumentStatus.WithFinanceTeam
                : TripDocumentStatus.PartiallyConfirmed;
            _tripRepository.Update(trip);

            foreach (var item in dto.CollectorDocuments)
            {
                var directoryPath = TripStoragePathHelper.CollectorDocument(dto.TripId, item.DocumentId.ToString());
                var uploadedPaths = await _mediaService.UploadImagesAsync(item.Files ?? [], directoryPath);
                newlyUploadedPaths.AddRange(uploadedPaths);

                var previousPaths = await UpsertConfirmedTripDocumentAsync(
                    dto.TripId,
                    dto.FinanceCollectorId,
                    item.DocumentId,
                    uploadedPaths,
                    item.Status,
                    item.Note);

                pathsToCleanupAfterCommit.AddRange(previousPaths);
            }

            await _tripRepository.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            await BestEffortRemoveAsync(newlyUploadedPaths);
            throw;
        }

        await BestEffortRemoveAsync(pathsToCleanupAfterCommit);
    }

    private async Task<IReadOnlyList<string>> UpsertConfirmedTripDocumentAsync(
        long tripId,
        long financeCollectorId,
        long documentId,
        IReadOnlyList<string> newPaths,
        CollectorDocumentStatus status,
        string? note)
    {
        var document = await _documentRepository.GetByIdOrThrowAsync(documentId);
        if (document.EntityId != DocumentEntityType.TRIP)
        {
            throw new ValidationException("documents.invalid_entity_for_trip_collection");
        }
        if (document.Status != DocumentStatus.ACTIVE)
        {
            throw new ValidationException("documents.inactive");
        }

        var existing = await _tripDocumentRepository
            .GetCurrentByTripAndDocumentAsync(tripId, documentId);

        var previous = existing?.Paths?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList()
                       ?? new List<string>();

        var hasNewFiles = newPaths.Count > 0;
        var priorPaths = existing?.Paths ?? Array.Empty<string>();
        var finalPaths = hasNewFiles ? newPaths.ToArray() : priorPaths;

        if (existing is null)
        {
            await _tripDocumentRepository.AddAsync(new TripDocument
            {
                TripId = tripId,
                DocumentId = documentId,
                Paths = finalPaths,
                Status = status,
                Note = note,
                AddedBy = financeCollectorId
            });
        }
        else
        {
            existing.Paths = finalPaths;
            existing.Status = status;
            existing.Note = note;
            _tripDocumentRepository.Update(existing);
        }

        return hasNewFiles ? previous : Array.Empty<string>();
    }

    private async Task BestEffortRemoveAsync(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                continue;
            try
            {
                await _mediaService.RemoveAsync(path);
            }
            catch
            {
                // Best-effort cleanup: a failure here must not crash the request.
            }
        }
    }
}
