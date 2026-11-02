using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Collector.Actions.Trips;

public class CollectorListTripsAction
{
    private static readonly int[] CollectorVisibleStatuses =
    {
        (int)TripStatus.Completed,
    };

    private readonly ITripRepository _tripRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CollectorListTripsAction(
        ITripRepository tripRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _tripRepository = tripRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<TripDto>> ExecuteAsync(CollectorListTripsDto dto)
    {
        var requestedStatus = dto.Status.HasValue ? (int?)dto.Status.Value : null;
        if (requestedStatus.HasValue && !CollectorVisibleStatuses.Contains(requestedStatus.Value))
        {
            return new ListingDto<TripDto>
            {
                Data = new List<TripDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, 0)
            };
        }

        var (trips, totalCount) = await _tripRepository.ListAsync(
            contractId: dto.ContractId,
            contractReferenceNumber: dto.ContractReferenceNumber,
            companyId: dto.CompanyId,
            matchingCompanyIds: null,
            companyName: dto.CompanyName,
            driverName: dto.DriverName,
            plate: dto.Plate,
            vehicleId: dto.VehicleId,
            vehicleTypeId: dto.VehicleTypeId,
            sectorIds: null,
            driverId: dto.DriverId,
            leadIds: null,
            isExternalTrip: null,
            contractorIds: dto.ContractorIds,
            hasContractor: dto.HasContractor,
            status: requestedStatus,
            tripDocumentStatuses: dto.TripDocumentStatuses,
            overNightCostBearer: null,
            dispatchingMode: null,
            statuses: CollectorVisibleStatuses,
            startDateFrom: dto.StartDateFrom,
            startDateTo: dto.StartDateTo,
            currentDriverStatus: null,
            lifecycleActor: null,
            search: dto.Search,
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
