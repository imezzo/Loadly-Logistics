using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class PatchContractBillingSnapshotCommercialTermsAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PatchContractBillingSnapshotCommercialTermsAction(
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
        PatchContractBillingSnapshotCommercialTermsDto dto)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsOrThrowAsync(dto.SnapshotId);
        ContractBillingSnapshotInvoicingProgress.EnsureDraftForMutation(snapshot.Status);

        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new ValidationException("contract_billing_snapshot.commercial_override_reason_required");

        if (dto.MonthlyRental is null && dto.ExtraWorkingDayFees is null && dto.OverConsumption is null)
            throw new ValidationException("contract_billing_snapshot.commercial_override_rate_required");

        if (dto.SeatRootSlotIds.Count == 0)
            throw new ValidationException("contract_billing_snapshot.commercial_override_seats_required");

        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(snapshot.ContractId);
        decimal? resolvedDailyValue = null;
        if (dto.MonthlyRental.HasValue)
        {
            var monthlyWorkingDays = ContractCalculationHelper.ResolveMonthlyWorkingDays(
                contract.MonthlyWorkingDays,
                contract.ContractType);
            resolvedDailyValue = ContractCalculationHelper.CalculateDailyValue(
                dto.MonthlyRental,
                monthlyWorkingDays,
                contract.ContractType);
            if (resolvedDailyValue is not > 0m)
                throw new ValidationException("contracts.contract_vehicle_type_monthly_rental_invalid");
        }
        var slots = contract.ContractSlots.ToList();
        var seatRootSlotIds = dto.SeatRootSlotIds
            .Select(id => ContractBillingSnapshotCommercialTermsResolver.ResolveSeatRootSlotId(id, slots))
            .Distinct()
            .ToList();

        ContractBillingSnapshotCommercialTermsResolver.ValidateEffectiveRange(
            snapshot,
            dto.EffectiveStart,
            dto.EffectiveEnd);

        var reason = dto.Reason.Trim();
        var addedBy = _currentUserService.GetCurrentUserId();
        foreach (var seatRootSlotId in seatRootSlotIds)
            UpsertCommercialOverride(snapshot, seatRootSlotId, dto, resolvedDailyValue, reason, addedBy);

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

    private static void UpsertCommercialOverride(
        ContractBillingSnapshot snapshot,
        long seatRootSlotId,
        PatchContractBillingSnapshotCommercialTermsDto dto,
        decimal? resolvedDailyValue,
        string reason,
        long? addedBy)
    {
        var existing = snapshot.CommercialOverrides
            .FirstOrDefault(row => row.DeletedAt == null && row.SeatRootSlotId == seatRootSlotId);

        if (existing is null)
        {
            snapshot.CommercialOverrides.Add(new ContractBillingSnapshotCommercialOverride
            {
                Snapshot = snapshot,
                SnapshotId = snapshot.Id,
                SeatRootSlotId = seatRootSlotId,
                EffectiveStart = dto.EffectiveStart,
                EffectiveEnd = dto.EffectiveEnd,
                DailyValue = resolvedDailyValue,
                ExtraWorkingDayFees = dto.ExtraWorkingDayFees,
                OverConsumption = dto.OverConsumption,
                OverrideReason = reason,
                AddedBy = addedBy
            });
            return;
        }

        existing.EffectiveStart = dto.EffectiveStart;
        existing.EffectiveEnd = dto.EffectiveEnd;
        if (dto.MonthlyRental.HasValue)
            existing.DailyValue = resolvedDailyValue;
        existing.ExtraWorkingDayFees = dto.ExtraWorkingDayFees;
        existing.OverConsumption = dto.OverConsumption;
        existing.OverrideReason = reason;
    }
}

public class PatchContractBillingSnapshotCommercialTermsDto
{
    public long SnapshotId { get; set; }
    public IReadOnlyList<long> SeatRootSlotIds { get; set; } = [];
    public DateOnly EffectiveStart { get; set; }
    public DateOnly EffectiveEnd { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public decimal? OverConsumption { get; set; }
    public string? Reason { get; set; }
}
