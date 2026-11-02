using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class PatchContractBillingSnapshotCalendarDayTripCountAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchContractBillingSnapshotCalendarDayTripCountAction(
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
        PatchContractBillingSnapshotCalendarDayTripCountDto dto)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        ContractBillingSnapshotInvoicingProgress.EnsureDraftForMutation(snapshot.Status);

        var day = await _snapshotRepository.GetCalendarDayAsync(dto.SnapshotId, dto.CalendarDayId)
            ?? throw new NotFoundException("contract_billing_snapshot.calendar_day_not_found", dto.CalendarDayId);

        day.TripIds = dto.TripIds.Distinct().OrderBy(tripId => tripId).ToArray();
        day.TripCountSource = ContractBillingSnapshotDispositionSource.Manual;
        if (dto.Reason is not null)
            day.OverrideReason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim();

        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(snapshot.ContractId);
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
