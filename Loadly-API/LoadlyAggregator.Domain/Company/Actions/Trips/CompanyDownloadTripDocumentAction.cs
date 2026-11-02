using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyDownloadTripDocumentAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMediaService _mediaService;

    public CompanyDownloadTripDocumentAction(
        ITripRepository tripRepository,
        IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mediaService = mediaService;
    }

    public async Task<string> ExecuteAsync(CompanyDownloadTripDocumentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Path))
            throw new ValidationException(TripMessageKeys.DocumentPathRequired);

        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        if (trip.Contract?.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

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
