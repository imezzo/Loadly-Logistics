using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Trips;

/// <summary>
/// Returns a trip for a collector. Only trips that have reached a trip status where document
/// collection is allowed, and whose <see cref="TripDocumentStatus"/> is still before
/// <see cref="TripDocumentStatus.WithFactoryCollector"/>, are visible (matches collector listing).
/// </summary>
public class CollectorGetTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CollectorGetTripAction(ITripRepository tripRepository, IMapper mapper, IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<TripDto> ExecuteAsync(CollectorGetTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);

        if (trip.Status is not (TripStatus.InProgress or TripStatus.Completed or TripStatus.Scheduled))
        {
            throw new NotFoundException("trips.not_found_with_id", dto.Id);
        }

        if (!(trip.TripDocumentStatus < TripDocumentStatus.WithFactoryCollector))
        {
            throw new NotFoundException("trips.not_found_with_id", dto.Id);
        }

        var result = _mapper.Map<TripDto>(trip);
        result.EnrichTripDocuments(_mediaService);
        return result;
    }
}
