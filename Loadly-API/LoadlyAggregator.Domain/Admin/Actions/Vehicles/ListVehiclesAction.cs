using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class ListVehiclesAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public ListVehiclesAction(
        IVehicleRepository vehicleRepository,
        IVehicleTypeRepository vehicleTypeRepository,
        IContractRepository contractRepository,
        IMediaService mediaService,
        IMapper mapper,
        ILanguageService languageService)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
        _contractRepository = contractRepository;
        _mediaService = mediaService;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<ListingDto<VehicleDto>> ExecuteAsync(ListVehiclesDto dto)
    {
        bool? isAvailableTail = dto.IsAvailableTail;
        List<long>? vehicleTypeIds = dto.VehicleTypeIds;
        List<long>? headLookupForTailTypeIds = null;

        if (dto.PickVehiclesOrTheirHeads && dto.VehicleTypeIds?.Count > 0)
        {
            var lineVehicleTypes = (await _vehicleTypeRepository.FindAsync(vt => dto.VehicleTypeIds.Contains(vt.Id))).ToList();

            headLookupForTailTypeIds = lineVehicleTypes
                .Where(vt => vt.VehicleUnitType == VehicleUnitType.Tail)
                .Select(vt => vt.Id)
                .ToList();

            vehicleTypeIds = lineVehicleTypes
                .Where(vt => vt.VehicleUnitType != VehicleUnitType.Tail)
                .Select(vt => vt.Id)
                .ToList();

            if (headLookupForTailTypeIds.Count > 0)
                isAvailableTail = null;

            if (headLookupForTailTypeIds.Count == 0) headLookupForTailTypeIds = null;
            if (vehicleTypeIds.Count == 0) vehicleTypeIds = null;
        }

        long? contractIdFilter = dto.ContractId;
        bool? isAssignedToContractFilter = dto.IsAssignedToContract;

        if (dto.ContractId.HasValue)
        {
            var contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId.Value);
            if (contract.AutoAssignMode)
            {
                contractIdFilter = null;
                isAssignedToContractFilter = false;
            }
        }

        var (vehicles, totalCount) = await _vehicleRepository.ListWithChildrenAsync(
            dto.Search,
            contractIdFilter,
            isAssignedToContractFilter,
            dto.ColorId,
            dto.CapacityUnitId,
            dto.VehicleTypeId,
            vehicleTypeIds,
            headLookupForTailTypeIds,
            dto.VehicleUnitType,
            dto.VehicleTypeCategories,
            dto.Status,
            isAvailableTail,
            dto.DriverId,
            dto.ContractorIds,
            dto.HasContractor,
            dto.HasDriver,
            dto.SectorIds,
            dto.CompanyIds,
            dto.FacilityIds,
            dto.FacilityTypes,
            dto.OrderByCompletedTripsDescending,
            dto.IsWaitingForLoading,
            dto.Page,
            dto.PageSize);

        if (vehicles.Count == 0)
        {
            return new ListingDto<VehicleDto>
            {
                Data = new List<VehicleDto>(),
                Additional = ExportListingMetadata.From(VehicleExportColumns.All, _languageService),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var data = vehicles
            .Select(vehicle =>
            {
                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                vehicleDto.CompletedTripsCount = vehicle.Trips.Count(t => IsCompletedTripWithinPeriod(t, dto.TripsStartDateFrom, dto.TripsEndDateTo));
                foreach (var doc in vehicleDto.VehicleDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
                {
                    var documentNameEn = doc.Document!.NameEn.ToLower().Replace(" ", "-");
                    doc.FileUrl = _mediaService.Read($"vehicle/{vehicleDto.Id}/{documentNameEn}/{doc.Path}");
                }
                return vehicleDto;
            })
            .ToList();

        return new ListingDto<VehicleDto>
        {
            Data = data,
            Additional = ExportListingMetadata.From(VehicleExportColumns.All, _languageService),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }

    private static bool IsCompletedTripWithinPeriod(
        Domain.Common.Entities.Trip trip,
        DateTime? tripsStartDateFrom,
        DateTime? tripsEndDateTo)
    {
        bool isCompleted = trip.Status == TripStatus.Completed;
        bool isAfterOrAtStart = !tripsStartDateFrom.HasValue || (trip.StartDate.HasValue && trip.StartDate.Value >= tripsStartDateFrom.Value);
        bool isBeforeOrAtEnd = !tripsEndDateTo.HasValue || (trip.EndDate.HasValue && trip.EndDate.Value <= tripsEndDateTo.Value);
        return isCompleted && isAfterOrAtStart && isBeforeOrAtEnd;
    }
}
