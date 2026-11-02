using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractBillingSnapshotAction
{
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly IContractRepository _contractRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly ILanguageService _languageService;

    public GetContractBillingSnapshotAction(
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

    public async Task<ContractBillingSnapshotDetailDto> GetByIdAsync(long snapshotId)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(snapshotId);
        return await EnrichAsync(ContractBillingSnapshotDetailDto.FromEntity(snapshot));
    }

    private async Task<ContractBillingSnapshotDetailDto> EnrichAsync(ContractBillingSnapshotDetailDto detail)
    {
        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(detail.ContractId);
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
        await ContractBillingCalendarDayTripCosts.ApplyAsync(detail.Calculation, contract, _tripRepository);
        return detail;
    }
}
