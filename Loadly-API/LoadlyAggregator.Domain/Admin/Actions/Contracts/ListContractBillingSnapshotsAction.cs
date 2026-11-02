using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class ListContractBillingSnapshotsAction
{
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly IContractRepository _contractRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly ILanguageService _languageService;

    public ListContractBillingSnapshotsAction(
        IContractBillingSnapshotRepository snapshotRepository,
        IContractRepository contractRepository,
        ITripRepository tripRepository,
        IVehicleFacilityRepository facilityRepository,
        ILanguageService languageService)
    {
        _snapshotRepository = snapshotRepository;
        _contractRepository = contractRepository;
        _tripRepository = tripRepository;
        _facilityRepository = facilityRepository;
        _languageService = languageService;
    }

    public async Task<ListingDto<ContractBillingSnapshotDetailDto>> ExecuteAsync(
        ListContractBillingSnapshotsDto dto)
    {
        var (snapshots, totalCount) = await _snapshotRepository.ListAsync(
            dto.Search,
            dto.ContractId,
            dto.Status,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.Page,
            dto.PageSize);

        var details = snapshots.Select(ContractBillingSnapshotDetailDto.FromEntity).ToList();
        await EnrichVehiclesAsync(details);

        return new ListingDto<ContractBillingSnapshotDetailDto>
        {
            Data = details,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }

    private async Task EnrichVehiclesAsync(List<ContractBillingSnapshotDetailDto> details)
    {
        var contracts = new Dictionary<long, Contract>();
        foreach (var detail in details)
        {
            if (!contracts.TryGetValue(detail.ContractId, out var contract))
            {
                contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(detail.ContractId);
                contracts[detail.ContractId] = contract;
            }

            bool isArabic = _languageService.IsArabic();
            var companyDetail = contract.Company?.CompanyDetail;
            detail.ContractName = isArabic ? contract.NameAr : contract.NameEn;
            detail.ReferenceNumber = contract.ReferenceNumber;
            detail.CompanyId = contract.CompanyId;
            detail.CompanyName = isArabic
                ? companyDetail?.NameAr ?? string.Empty
                : companyDetail?.NameEn ?? string.Empty;
            detail.ConsumptionType = contract.ConsumptionType;
            ContractSlotBillingVehiclesMapper.ApplyVehicles(
                detail.Calculation,
                contract,
                detail.PeriodStart,
                detail.PeriodEnd);
            await CalendarDayFacilityNameHelper.ApplyAsync(detail.Calculation, _facilityRepository);
        }

        await ContractBillingCalendarDayTripCosts.ApplyAsync(
            details.Select(detail => (detail.Calculation, contracts[detail.ContractId])).ToList(),
            _tripRepository);
    }
}
