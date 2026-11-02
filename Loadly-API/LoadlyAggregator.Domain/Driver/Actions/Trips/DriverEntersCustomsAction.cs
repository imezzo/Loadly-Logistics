using System.Text.Json;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Entities.EventPayloads;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverEntersCustomsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IRecordingRepository _recordingRepository;
    private readonly IMediaService _mediaService;

    public DriverEntersCustomsAction(
        ITripRepository tripRepository,
        IRecordingRepository recordingRepository,
        IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _recordingRepository = recordingRepository;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(DriverEntersCustomsDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        TripValidator.ValidateDriverOwnsTrip(trip, dto.DriverId);

        if (trip.CustomsStatus >= TripCustomsStatus.UnderInspection)
            return;

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
            AddedBy = dto.DriverId,
            AddedByModelType = "Driver",
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
