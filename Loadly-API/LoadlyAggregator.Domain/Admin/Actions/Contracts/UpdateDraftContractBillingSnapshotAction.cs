using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class UpdateDraftContractBillingSnapshotAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly CalculateContractSlotChargesAction _calculateContractSlotChargesAction;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDraftContractBillingSnapshotAction(
        IContractRepository contractRepository,
        IContractBillingSnapshotRepository snapshotRepository,
        CalculateContractSlotChargesAction calculateContractSlotChargesAction,
        ITripRepository tripRepository,
        IVehicleFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _snapshotRepository = snapshotRepository;
        _calculateContractSlotChargesAction = calculateContractSlotChargesAction;
        _tripRepository = tripRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(long SnapshotId, string Name, CalculateIncludedContractSlotChargesResultDto Calculation)> ExecuteAsync(
        UpdateDraftContractBillingSnapshotDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(dto.ContractId);
        var tracked = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        if (tracked.ContractId != dto.ContractId)
            throw new NotFoundException("contract_billing_snapshot.not_found", dto.SnapshotId);
        ContractBillingSnapshotInvoicingProgress.EnsureDraftForMutation(tracked.Status);

        if (await _snapshotRepository.HasOverlappingPeriodForContractAsync(
                dto.ContractId,
                dto.PeriodStart,
                dto.PeriodEnd,
                excludeSnapshotId: tracked.Id))
            throw new ValidationException("contract_billing_snapshot.period_overlap");

        var calculation = await _calculateContractSlotChargesAction.ExecuteAsync(new CalculateContractSlotChargesDto
        {
            ContractId = dto.ContractId,
            ContractSlotId = dto.ContractSlotId,
            PeriodStart = dto.PeriodStart,
            PeriodEnd = dto.PeriodEnd
        });
        ContractSlotInvoiceReadinessValidator.ValidateForTripsInvoice(
            contract,
            dto.PeriodStart,
            dto.PeriodEnd,
            calculation);

        tracked.PeriodStart = dto.PeriodStart;
        tracked.PeriodEnd = dto.PeriodEnd;
        ContractBillingSnapshotDraftRefresher.ApplyName(tracked, contract, dto.Name);
        await ContractBillingSnapshotDraftRefresher.RefreshPreservingManualOverridesAsync(
            tracked,
            calculation,
            contract,
            _tripRepository);

        await _unitOfWork.SaveChangesAsync();
        var persistedCalculation = ContractBillingSnapshotReader.ToCalculationDto(tracked);
        ContractSlotBillingVehiclesMapper.ApplyVehicles(
            persistedCalculation,
            contract,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.ContractSlotId);
        await CalendarDayFacilityNameHelper.ApplyAsync(persistedCalculation, _facilityRepository);
        await ContractBillingCalendarDayTripCosts.ApplyAsync(persistedCalculation, contract, _tripRepository);
        return (tracked.Id, tracked.Name, persistedCalculation);
    }
}
