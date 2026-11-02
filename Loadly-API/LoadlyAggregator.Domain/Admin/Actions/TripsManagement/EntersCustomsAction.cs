using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.TripsManagement;

public class EntersCustomsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;
    private readonly ICurrentUserService _currentUserService;

    public EntersCustomsAction(
        ITripRepository tripRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(EntersCustomsDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        _ = trip.DriverId ?? throw new ValidationException(TripMessageKeys.VehicleMustHaveAssignedDriver);

        if (trip.CustomsStatus >= TripCustomsStatus.UnderInspection)
        {
            return;
        }

        TripValidator.ValidateDriverEntersCustomsEligibility(trip);

        List<string>? customsDocumentsPaths = null;
        if (dto.CustomsDocuments != null && dto.CustomsDocuments.Any(i => i.Length > 0))
        {
            customsDocumentsPaths = await _mediaService.UploadImagesAsync(
                dto.CustomsDocuments,
                TripStoragePathHelper.CustomsDocuments(dto.TripId));
        }

        var recording = new Recording
        {
            RecordableType = RecordableType.TRIP,
            RecordableId = dto.TripId,
            RecordableEventType = RecordableEventType.TRIP_CUSTOMS_ENTERED,
            AddedBy = _currentUserService.GetCurrentUserId(),
            AddedByModelType = "Admin",
            Data = JsonSerializer.SerializeToDocument(new DriverEntersCustomsEventData
            {
                Note = dto.Note,
                CustomsDocumentsPaths = customsDocumentsPaths,
                Location = dto.CurrentLocation
            })
        };
        await _recordingRepository.AddAsync(recording);

        trip.CustomsStatus = TripCustomsStatus.UnderInspection;
        _tripRepository.Update(trip);

        await _tripRepository.SaveChangesAsync();
    }
}
