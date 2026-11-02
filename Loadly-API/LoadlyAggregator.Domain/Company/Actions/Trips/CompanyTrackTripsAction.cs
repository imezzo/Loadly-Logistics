using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyTrackTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;

    public CompanyTrackTripsAction(ITripRepository tripRepository, IMapper mapper)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<TripTrackItemDto>> ExecuteAsync(CompanyTrackTripsDto dto)
    {
        var trips = await _tripRepository.ListActiveTripsWithLocationAsync(
            dto.CompanyId,
            dto.ContractReferenceNumber,
            null,
            null);

        var data = trips.Select(trip => _mapper.Map<TripTrackItemDto>(trip)).ToList();
        return new ListingDto<TripTrackItemDto> { Data = data };
    }
}
