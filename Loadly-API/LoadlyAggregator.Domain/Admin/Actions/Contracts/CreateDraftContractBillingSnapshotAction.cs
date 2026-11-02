using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class CreateDraftContractBillingSnapshotAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _snapshotRepository;
    private readonly CalculateContractSlotChargesAction _calculateContractSlotChargesAction;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDraftContractBillingSnapshotAction(
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
        CalculateContractSlotChargesDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(dto.ContractId);

        if (await _snapshotRepository.HasOverlappingPeriodForContractAsync(
                dto.ContractId,
                dto.PeriodStart,
                dto.PeriodEnd))
            throw new ValidationException("contract_billing_snapshot.period_overlap");

        var calculation = await _calculateContractSlotChargesAction.ExecuteAsync(dto);
        ContractSlotInvoiceReadinessValidator.ValidateForTripsInvoice(
            contract,
            dto.PeriodStart,
            dto.PeriodEnd,
            calculation);

        var snapshot = ContractBillingSnapshotMaterializer.FromCalculation(
            dto.ContractId,
            contract.ContractType,
            dto.PeriodStart,
            dto.PeriodEnd,
            calculation,
            ContractBillingSnapshotStatus.Draft,
            ContractBillingSnapshotNameFormatter.Resolve(contract, dto.Name));
        await _snapshotRepository.AddAsync(snapshot);
        await _unitOfWork.SaveChangesAsync();
        var persistedCalculation = ContractBillingSnapshotReader.ToCalculationDto(snapshot);
        ContractSlotBillingVehiclesMapper.ApplyVehicles(
            persistedCalculation,
            contract,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.ContractSlotId);
        await CalendarDayFacilityNameHelper.ApplyAsync(persistedCalculation, _facilityRepository);
        await ContractBillingCalendarDayTripCosts.ApplyAsync(persistedCalculation, contract, _tripRepository);
        return (snapshot.Id, snapshot.Name, persistedCalculation);
    }
}
