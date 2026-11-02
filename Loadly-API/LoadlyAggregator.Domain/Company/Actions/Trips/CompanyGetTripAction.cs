using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyGetTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CompanyGetTripAction(ITripRepository tripRepository, IMapper mapper, IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<TripDto> ExecuteAsync(CompanyGetTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.Id);

        if (trip.Contract?.CompanyId != dto.CompanyId)
        {
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);
        }

        var result = _mapper.Map<TripDto>(trip);
        result.EnrichTripDocuments(_mediaService);
        return result;
    }
}
