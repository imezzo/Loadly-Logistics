using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;


namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class DownloadTripDocumentAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMediaService _mediaService;

    public DownloadTripDocumentAction(
        ITripRepository tripRepository,
        IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mediaService = mediaService;
    }

    public async Task<string> ExecuteAsync(DownloadTripDocumentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Path))
            throw new ValidationException(TripMessageKeys.DocumentPathRequired);

        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.TripId);

        var normalizedPath = dto.Path.Trim().TrimStart('/');
        var expectedPrefix = $"trips/{dto.TripId}/";
        if (!normalizedPath.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException(TripMessageKeys.DocumentPathInvalid);

        if (normalizedPath.Contains(".."))
            throw new ValidationException(TripMessageKeys.DocumentPathInvalid);

        var url = _mediaService.Read(normalizedPath);
        if (string.IsNullOrEmpty(url))
            throw new NotFoundException(TripMessageKeys.DocumentNotFound);

        return url;
    }
}
