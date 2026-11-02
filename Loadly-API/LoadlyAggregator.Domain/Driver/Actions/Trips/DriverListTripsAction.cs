using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Driver.Actions.Trips;

public class DriverListTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public DriverListTripsAction(ITripRepository tripRepository, IMapper mapper, IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<TripDto>> ExecuteAsync(DriverListTripsDto dto)
    {
        var (trips, totalCount) = await _tripRepository.ListAsync(
            contractId: null,
            contractReferenceNumber: null,
            companyId: null,
            matchingCompanyIds: null,
            companyName: null,
            driverName: null,
            plate: null,
            vehicleId: null,
            vehicleTypeId: null,
            sectorIds: null,
            driverId: dto.DriverId,
            leadIds: null,
            isExternalTrip: null,
            contractorIds: null,
            hasContractor: null,
            status: dto.Status.HasValue ? (int?)dto.Status.Value : null,
            tripDocumentStatuses: dto.TripDocumentStatuses,
            overNightCostBearer: null,
            dispatchingMode: null,
            statuses: new[] { (int)TripStatus.Scheduled, (int)TripStatus.InProgress, (int)TripStatus.Completed },
            startDateFrom: dto.StartDateFrom,
            startDateTo: dto.StartDateTo,
            currentDriverStatus: null,
            lifecycleActor: null,
            search: null,
            page: dto.Page,
            pageSize: dto.PageSize);

        var tripDtos = trips.Select(trip => _mapper.Map<TripDto>(trip)).ToList();
        tripDtos.EnrichAll(_mediaService);

        return new ListingDto<TripDto>
        {
            Data = tripDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
