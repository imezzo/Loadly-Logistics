using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class ListTripsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyDetailRepository _companyDetailRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;
    private readonly ILanguageService _languageService;

    public ListTripsAction(
        ITripRepository tripRepository,
        ICompanyDetailRepository companyDetailRepository,
        IMapper mapper,
        IMediaService mediaService,
        ILanguageService languageService)
    {
        _tripRepository = tripRepository;
        _companyDetailRepository = companyDetailRepository;
        _mapper = mapper;
        _mediaService = mediaService;
        _languageService = languageService;
    }

    public async Task<ListingDto<TripDto>> ExecuteAsync(ListTripsDto dto)
    {
        List<long>? matchingCompanyIds = null;
        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            var searchTerm = dto.Search.Trim();
            var matchingCompanyDetails = await _companyDetailRepository.FindAsync(cd =>
                cd.NameEn.ToLower().Contains(searchTerm.ToLower()) ||
                (cd.NameAr != null && cd.NameAr.ToLower().Contains(searchTerm.ToLower()))
            );
            matchingCompanyIds = matchingCompanyDetails.Select(cd => cd.CompanyId).ToList();
        }

        var visibleStatuses = Enum.GetValues<TripStatus>().Where(status => status != TripStatus.CompanyDraft).Select(status => (int)status).ToArray();
        var (trips, totalCount) = await _tripRepository.ListAsync(
            dto.ContractId,
            dto.ContractReferenceNumber,
            dto.CompanyId,
            matchingCompanyIds,
            companyName: null,
            driverName: null,
            plate: null,
            dto.VehicleId,
            dto.VehicleTypeId,
            dto.SectorIds,
            dto.DriverId,
            dto.LeadIds,
            dto.IsExternalTrip,
            dto.ContractorIds,
            dto.HasContractor,
            dto.Status.HasValue ? (int?)dto.Status.Value : null,
            dto.TripDocumentStatuses,
            dto.OverNightCostBearer.HasValue ? (int?)dto.OverNightCostBearer.Value : null,
            dto.DispatchingMode.HasValue ? (int?)dto.DispatchingMode.Value : null,
            statuses: dto.Statuses is { Count: > 0 }
                ? dto.Statuses.Select(s => (int)s).ToArray()
                : visibleStatuses,
            dto.StartDateFrom,
            dto.StartDateTo,
            dto.CurrentDriverStatus,
            dto.LifecycleActor,
            dto.Search,
            dto.Page,
            dto.PageSize,
            dto.IsBackfill,
            dto.DestinationIds,
            isPushedToContractors: dto.IsPushedToContractors,
            hasContractorInvoice: dto.HasContractorInvoice,
            isCreatedByCompany: dto.IsCreatedByCompany,
            contractVehicleTypeId: dto.ContractVehicleTypeId,
            contractTypeId: dto.ContractTypeId,
            financialDateFrom: dto.FinancialDateFrom,
            financialDateTo: dto.FinancialDateTo);

        var tripList = trips.ToList();
        var tripDtos = tripList.Select(trip => _mapper.Map<TripDto>(trip)).ToList();
        tripDtos.EnrichAllContractConditions(tripList, _mediaService);
        tripDtos.EnrichAll(_mediaService);

        return new ListingDto<TripDto>
        {
            Data = tripDtos,
            Additional = ExportListingMetadata.From(TripExportColumns.All, _languageService),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
