using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class TrackTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;

    public TrackTripsAction(ITripRepository tripRepository, IMapper mapper)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<TripTrackItemDto>> ExecuteAsync(TrackTripsDto dto)
    {
        var trips = await _tripRepository.ListActiveTripsWithLocationAsync(
            dto.CompanyId,
            dto.ContractReferenceNumber,
            dto.DriverId,
            dto.VehicleId);

        var data = trips.Select(trip => _mapper.Map<TripTrackItemDto>(trip)).ToList();
        return new ListingDto<TripTrackItemDto> { Data = data };
    }
}
