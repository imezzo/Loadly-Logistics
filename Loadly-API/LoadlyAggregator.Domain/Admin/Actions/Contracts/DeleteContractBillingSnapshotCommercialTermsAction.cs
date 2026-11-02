using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class DeleteContractBillingSnapshotCommercialTermsAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractBillingSnapshotCommercialTermsAction(
        IContractRepository contractRepository,
        IContractBillingSnapshotRepository snapshotRepository,
        ITripRepository tripRepository,
        IVehicleFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _snapshotRepository = snapshotRepository;
        _tripRepository = tripRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(long SnapshotId, string Name, CalculateIncludedContractSlotChargesResultDto Calculation)> ExecuteAsync(
        DeleteContractBillingSnapshotCommercialTermsDto dto)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        ContractBillingSnapshotInvoicingProgress.EnsureDraftForMutation(snapshot.Status);

        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(snapshot.ContractId);
        var seatRootSlotId = ContractBillingSnapshotCommercialTermsResolver.ResolveSeatRootSlotId(
            dto.SeatRootSlotId,
            contract.ContractSlots.ToList());

        var existing = snapshot.CommercialOverrides
            .FirstOrDefault(row => row.DeletedAt == null && row.SeatRootSlotId == seatRootSlotId)
            ?? throw new NotFoundException("contract_billing_snapshot.commercial_override_not_found", seatRootSlotId);

        existing.SetDeletedAt(DateTime.UtcNow);

        await ContractBillingSnapshotChargeRebuildHelper.RebuildDraftChargesAsync(
            snapshot,
            contract,
            _tripRepository);

        await _unitOfWork.SaveChangesAsync();
        var calculation = ContractBillingSnapshotReader.ToCalculationDto(snapshot);
        ContractSlotBillingVehiclesMapper.ApplyVehicles(
            calculation,
            contract,
            snapshot.PeriodStart,
            snapshot.PeriodEnd);
        await CalendarDayFacilityNameHelper.ApplyAsync(calculation, _facilityRepository);
        await ContractBillingCalendarDayTripCosts.ApplyAsync(calculation, contract, _tripRepository);
        return (snapshot.Id, snapshot.Name, calculation);
    }
}

public class DeleteContractBillingSnapshotCommercialTermsDto
{
    public long SnapshotId { get; set; }
    public long SeatRootSlotId { get; set; }
}
