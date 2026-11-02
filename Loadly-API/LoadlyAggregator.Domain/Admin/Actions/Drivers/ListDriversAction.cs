using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class ListDriversAction
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public ListDriversAction(
        IDriverRepository driverRepository,
        IMediaService mediaService,
        IMapper mapper,
        ILanguageService languageService)
    {
        _driverRepository = driverRepository;
        _mediaService = mediaService;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<ListingDto<DriverDto>> ExecuteAsync(ListDriversDto dto)
    {
        var (drivers, totalCount) = await _driverRepository.ListAsync(
            dto.Search,
            dto.Status,
            dto.ContractorIds,
            dto.HasContractor,
            dto.IsAvailable,
            dto.LicenseCategory,
            dto.WorkingStatuses,
            dto.Page,
            dto.PageSize,
            dto.OrderByCompletedTripsDescending);

        var driverDtos = new List<DriverDto>();
        foreach (var driver in drivers)
        {
            var driverDto = _mapper.Map<DriverDto>(driver);

            driverDto.CompletedTripsCount = driver.Trips.Count(t => IsCompletedTripWithinPeriod(t, dto.TripsStartDateFrom, dto.TripsEndDateTo));

            if (driver.DriverDetail != null)
            {
                driverDto.DriverDetail = _mapper.Map<DriverDetailDto>(driver.DriverDetail);
            }

            var documents = driver.DriverDocuments;
            driverDto.Documents = _mapper.Map<List<DriverDocumentDto>>(documents);

            foreach (var doc in documents)
            {
                if (doc.Document != null && !string.IsNullOrWhiteSpace(doc.Path))
                {
                    var documentNameEn = doc.Document.NameEn.ToLower().Replace(" ", "-");
                    var fileUrl = _mediaService.Read($"driver/{driverDto.Id}/{documentNameEn}/{doc.Path}");
                    var docDto = driverDto.Documents.FirstOrDefault(d => d.Id == doc.Id);
                    if (docDto != null)
                    {
                        docDto.FileUrl = fileUrl;
                    }
                }
            }

            var vehicle = driver.Vehicles.FirstOrDefault(v => v.HeadVehicleId == null) ?? driver.Vehicles.FirstOrDefault();
            if (vehicle != null)
                driverDto.Vehicle = _mapper.Map<VehicleRelationDto>(vehicle);

            driverDtos.Add(driverDto);
        }

        return new ListingDto<DriverDto>
        {
            Data = driverDtos,
            Additional = ExportListingMetadata.From(DriverExportColumns.All, _languageService),
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
