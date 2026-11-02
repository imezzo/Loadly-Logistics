using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class CalculateContractSlotChargesAction
{
    private readonly IContractRepository _contractRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityVisitRepository _visitRepository;

    public CalculateContractSlotChargesAction(
        IContractRepository contractRepository,
        ITripRepository tripRepository,
        IVehicleFacilityVisitRepository visitRepository)
    {
        _contractRepository = contractRepository;
        _tripRepository = tripRepository;
        _visitRepository = visitRepository;
    }

    public async Task<CalculateIncludedContractSlotChargesResultDto> ExecuteAsync(
        CalculateContractSlotChargesDto dto)
    {
        if (dto.PeriodEnd < dto.PeriodStart)
            throw new ValidationException("validation.end_date_gte_start_date");

        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(dto.ContractId);
        if (!ContractCalculationHelper.IsMonthlyContract(contract.ContractType))
            throw new ValidationException("contracts.slot_charges_require_monthly_contract");

        var monthlyWorkingDays = ContractCalculationHelper.ResolveMonthlyWorkingDays(
            contract.MonthlyWorkingDays,
            contract.ContractType) ?? ContractCalculationHelper.DefaultMonthlyWorkingDays;

        var workingDayWindow = ContractWorkingDayWindow.Default;
        var slots = IncludedContractSlotChargesComputer.ResolveTargetSlots(
            contract,
            dto.ContractSlotId,
            dto.PeriodStart,
            dto.PeriodEnd);

        var kmOverageMode = contract.ContractType == ContractType.Included
            ? ContractCalculationHelper.ResolveIncludedKmOverageMode(contract.KmOverageMode)
            : ContractKmOverageMode.PooledAcrossSlots;

        var result = await IncludedContractSlotChargesComputer.ComputeAsync(
            contract.Id,
            slots,
            dto.PeriodStart,
            dto.PeriodEnd,
            monthlyWorkingDays,
            workingDayWindow,
            kmOverageMode,
            slot => slot,
            _tripRepository,
            _visitRepository,
            contract.ContractSlots.ToList(),
            contract.ContractVehicleTypes.ToList(),
            contract.ContractType);

        ContractSlotBillingVehiclesMapper.ApplyVehicles(result, slots);
        await ContractBillingCalendarDayTripCosts.ApplyConsumptionTotalsAsync([(result, contract)], _tripRepository);
        return result;
    }
}
