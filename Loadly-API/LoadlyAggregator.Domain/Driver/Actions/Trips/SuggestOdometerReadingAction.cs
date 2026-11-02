using System.Diagnostics;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class SuggestOdometerReadingAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IOdometerReadingExtractor _odometerReadingExtractor;
    private readonly IMediaService _mediaService;
    private readonly IOdometerScanLogRepository _odometerScanLogRepository;
    private readonly TimeProvider _timeProvider;

    public SuggestOdometerReadingAction(
        ITripRepository tripRepository,
        IOdometerReadingExtractor odometerReadingExtractor,
        IMediaService mediaService,
        IOdometerScanLogRepository odometerScanLogRepository,
        TimeProvider timeProvider)
    {
        _tripRepository = tripRepository;
        _odometerReadingExtractor = odometerReadingExtractor;
        _mediaService = mediaService;
        _odometerScanLogRepository = odometerScanLogRepository;
        _timeProvider = timeProvider;
    }

    public async Task<SuggestOdometerReadingResultDto> ExecuteAsync(SuggestOdometerReadingDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        TripValidator.ValidateDriverOwnsTrip(trip, dto.ModelId);

        var imageBytes = await dto.OdometerImage.ReadAsBytesAsync();
        var uploadFile = CreateFileUpload(imageBytes, dto.OdometerImage);
        var imagePath = await _mediaService.UploadAndGetPathAsync(
            uploadFile,
            TripStoragePathHelper.OdometerScans(dto.TripId));

        var stopwatch = Stopwatch.StartNew();
        OdometerReadingExtraction extraction;
        string? errorMessage = null;
        try
        {
            extraction = await _odometerReadingExtractor.ExtractAsync(
                imageBytes,
                dto.OdometerImage.ContentType);
        }
        catch (TimeoutException)
        {
            extraction = new OdometerReadingExtraction(null, null, null, null, false, null, null, _odometerReadingExtractor.Provider);
            errorMessage = TripMessageKeys.OdometerReadingExtractionFailed;
        }
        catch (InvalidOperationException)
        {
            extraction = new OdometerReadingExtraction(null, null, null, null, false, null, null, _odometerReadingExtractor.Provider);
            errorMessage = TripMessageKeys.OdometerReadingExtractionFailed;
        }

        stopwatch.Stop();

        var status = ResolveStatus(extraction, errorMessage);
        var log = BuildLog(dto, imagePath, extraction, status, errorMessage, (int)stopwatch.ElapsedMilliseconds);
        await _odometerScanLogRepository.AddAsync(log);
        await _odometerScanLogRepository.SaveChangesAsync();

        if (extraction.IsScreen)
            throw new ValidationException(TripMessageKeys.OdometerScreenCaptureNotAllowed);

        if (errorMessage is not null || extraction.Reading is null)
            throw new ValidationException(TripMessageKeys.OdometerReadingExtractionFailed);

        return new SuggestOdometerReadingResultDto
        {
            ScanLogId = log.Id,
            SuggestedReading = extraction.Reading.Value,
            Unit = extraction.Unit,
            IsDigital = extraction.IsDigital,
            ReadingQuality = extraction.ReadingQuality
        };
    }

    private static OdometerScanStatus ResolveStatus(OdometerReadingExtraction extraction, string? errorMessage)
    {
        if (errorMessage is not null)
            return OdometerScanStatus.Failure;

        if (extraction.IsScreen || extraction.Reading is null)
            return OdometerScanStatus.Failure;

        return OdometerScanStatus.Success;
    }

    private OdometerScanLog BuildLog(
        SuggestOdometerReadingDto dto,
        string imagePath,
        OdometerReadingExtraction extraction,
        OdometerScanStatus status,
        string? errorMessage,
        int latencyMs) =>
        new()
        {
            CreatedAt = AppUtcTime.UtcNow(_timeProvider),
            ModelId = dto.ModelId,
            ModelType = dto.ModelType,
            TripId = dto.TripId,
            ScanContext = dto.ScanContext,
            ImagePath = imagePath,
            SuggestedReading = extraction.Reading,
            Unit = extraction.Unit,
            IsDigital = extraction.IsDigital,
            ReadingQuality = extraction.ReadingQuality,
            IsScreen = extraction.IsScreen,
            Status = status,
            ErrorMessage = extraction.IsScreen
                ? TripMessageKeys.OdometerScreenCaptureNotAllowed
                : errorMessage,
            Provider = extraction.Provider,
            Model = extraction.ProviderModel,
            TotalTokens = extraction.TotalTokens,
            LatencyMs = latencyMs,
            RawResponse = extraction.RawResponse,
            CorrelationId = dto.CorrelationId,
            PreviousScanLogId = dto.PreviousScanLogId
        };

    private static FileUpload CreateFileUpload(byte[] bytes, FileUpload source) =>
        new()
        {
            FileName = source.FileName,
            ContentType = source.ContentType,
            Length = bytes.Length,
            Content = new MemoryStream(bytes)
        };
}
