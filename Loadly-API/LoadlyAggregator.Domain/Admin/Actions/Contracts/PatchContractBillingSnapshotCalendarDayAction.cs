using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class PatchContractBillingSnapshotCalendarDayAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PatchContractBillingSnapshotCalendarDayAction(
        IContractRepository contractRepository,
        IContractBillingSnapshotRepository snapshotRepository,
        ITripRepository tripRepository,
        IVehicleFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _contractRepository = contractRepository;
        _snapshotRepository = snapshotRepository;
        _tripRepository = tripRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<(long SnapshotId, string Name, CalculateIncludedContractSlotChargesResultDto Calculation)> ExecuteAsync(
        PatchContractBillingSnapshotCalendarDayDto dto)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        ContractBillingSnapshotInvoicingProgress.EnsureDraftForMutation(snapshot.Status);

        if (IncludedContractHalfWorkingDayRules.IsHalfWorking(dto.Disposition)
            && snapshot.ContractType != ContractType.Included)
            throw new ValidationException("contract_billing_snapshot.half_working_day_included_only");

        var day = await _snapshotRepository.GetCalendarDayAsync(dto.SnapshotId, dto.CalendarDayId)
            ?? throw new NotFoundException("contract_billing_snapshot.calendar_day_not_found", dto.CalendarDayId);

        day.Disposition = dto.Disposition;
        day.DispositionSource = ContractBillingSnapshotDispositionSource.Manual;
        day.OverrideReason = dto.Reason;
        day.ChangedBy = _currentUserService.GetCurrentUserId();
        day.ChangedAt = DateTime.UtcNow;
        await ApplyFacilityAsync(day, dto);

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

    private async Task ApplyFacilityAsync(
        ContractBillingSnapshotCalendarDay day,
        PatchContractBillingSnapshotCalendarDayDto dto)
    {
        if (dto.Disposition == ContractSlotDayDisposition.Skipped)
        {
            if (!dto.FacilityId.HasValue)
                throw new ValidationException("contract_billing_snapshot.facility_required_for_out_of_service");

            await AssignFacilityAsync(day, dto.FacilityId.Value);
            return;
        }

        if (dto.Disposition is ContractSlotDayDisposition.Working or ContractSlotDayDisposition.HalfWorking
            && dto.FacilityId.HasValue)
        {
            var facility = await _facilityRepository.GetByIdOrThrowAsync(dto.FacilityId.Value);
            if (facility.FacilityType != VehicleFacilityType.Parking)
                throw new ValidationException("contract_billing_snapshot.working_day_facility_must_be_parking");

            day.FacilityId = facility.Id;
            day.FacilityType = facility.FacilityType;
            day.VehicleFacility = facility;
            return;
        }

        day.FacilityId = null;
        day.FacilityType = null;
        day.VehicleFacility = null;
    }

    private async Task<VehicleFacility> AssignFacilityAsync(ContractBillingSnapshotCalendarDay day, long facilityId)
    {
        var facility = await _facilityRepository.GetByIdOrThrowAsync(facilityId);
        day.FacilityId = facility.Id;
        day.FacilityType = facility.FacilityType;
        day.VehicleFacility = facility;
        return facility;
    }
}

public class PatchContractBillingSnapshotCalendarDayDto
{
    public long SnapshotId { get; set; }
    public long CalendarDayId { get; set; }
    public ContractSlotDayDisposition Disposition { get; set; }
    public long? FacilityId { get; set; }
    public string? Reason { get; set; }
}
