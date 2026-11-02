using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class PreviewTripContractorPriceAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly ITripRepository _tripRepository;

    public PreviewTripContractorPriceAction(
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        ITripRepository tripRepository)
    {
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _tripRepository = tripRepository;
    }

    public async Task<PreviewTripContractorPriceResultDto> ExecuteAsync(PreviewTripContractorPriceDto dto)
    {
        var contract = await _contractRepository.GetByIdAsync(dto.ContractId)
            ?? throw new NotFoundException("contracts.not_found_with_id", dto.ContractId);

        Trip? trip = null;
        if (dto.TripId is > 0)
        {
            trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId.Value);
            if (trip.ContractId != dto.ContractId)
                throw new ValidationException(TripMessageKeys.TripContractIdMismatch);
        }

        var destinationId = dto.DestinationId ?? (trip != null ? TripContractDestinationHelper.GetCurrentContractDestinationId(trip) : null);
        var vehicleTypeId = dto.VehicleTypeId ?? trip?.ContractVehicleType?.VehicleTypeId;
        var vehicleId = dto.VehicleId ?? trip?.VehicleId;
        var actualCargoWeightTons = dto.ActualCargoWeightTons ?? trip?.ActualCargoWeightTons;

        if (!TripContractorPricingHelper.RequiresContractorPrice(contract))
        {
            return NotApplicable(contract.ConsumptionType);
        }

        if (contract.ConsumptionType is ContractConsumptionTypes.SPOT_PRICING
            or ContractConsumptionTypes.CARGO_PRICE_COMMISSION)
        {
            return new PreviewTripContractorPriceResultDto
            {
                ConsumptionType = contract.ConsumptionType,
                IsApplicable = true,
                FormulaValidationApplies = false,
                ManualEntryRequired = true
            };
        }

        TripValidator.ValidateDestinationRequired(contract, destinationId);

        ContractDestination? tripDestination = await TripContractDestinationHelper
            .GetOptionalContractDestinationForTripAsync(
                dto.ContractId,
                destinationId,
                _contractDestinationRepository);

        ContractVehicleType? contractVehicleType = null;
        if (vehicleTypeId is > 0)
        {
            contractVehicleType = await _contractVehicleTypeRepository
                    .GetByContractIdAndVehicleTypeIdAsync(dto.ContractId, vehicleTypeId.Value)
                ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", vehicleTypeId.Value);
        }
        else if (contract.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS)
        {
            throw new ValidationException(TripMessageKeys.ContractorPriceCalculationUnavailable);
        }

        var contractSlotId = TripContractSlotHelper.ResolveContractSlotIdForTrip(
            contract,
            vehicleId,
            tripStartUtc: trip?.StartDate);
        var pricingSource = ResolvePricingSource(contract, contractSlotId);

        if (contract.ConsumptionType == ContractConsumptionTypes.DESTINATION_FIXED_PRICE)
        {
            var expected = TripContractorPricingHelper.CalculateExpected(
                contract,
                actualCargoWeightTons,
                contractVehicleType,
                tripDestination,
                contractSlotId);

            if (!expected.HasValue || expected.Value <= 0)
                throw new ValidationException(TripMessageKeys.ContractorPriceCalculationUnavailable);

            return new PreviewTripContractorPriceResultDto
            {
                ConsumptionType = contract.ConsumptionType,
                IsApplicable = true,
                FormulaValidationApplies = true,
                ManualEntryRequired = false,
                ExpectedContractorPrice = RoundExpected(expected.Value),
                PricingSource = pricingSource,
                Breakdown = new PreviewTripContractorPriceBreakdownDto
                {
                    ContractorFixedTripPrice = tripDestination?.ContractorFixedTripPrice
                }
            };
        }

        if (contract.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS)
        {
            var breakdown = TripContractorPricingHelper.CalculateCargoWeightsBreakdown(
                contract,
                actualCargoWeightTons,
                contractVehicleType,
                tripDestination,
                contractSlotId);

            if (breakdown is null || breakdown.Value.Total <= 0)
                throw new ValidationException(TripMessageKeys.ContractorPriceCalculationUnavailable);

            var value = breakdown.Value;
            return new PreviewTripContractorPriceResultDto
            {
                ConsumptionType = contract.ConsumptionType,
                IsApplicable = true,
                FormulaValidationApplies = true,
                ManualEntryRequired = false,
                ExpectedContractorPrice = RoundExpected(value.Total),
                PricingSource = pricingSource,
                Breakdown = new PreviewTripContractorPriceBreakdownDto
                {
                    ActualCargoWeightTons = value.ActualCargoWeightTons,
                    MinBillableWeightTons = value.MinBillableWeightTons,
                    BillableTons = value.BillableTons,
                    ContractorPricePerTon = value.ContractorPricePerTon,
                    BaseCost = value.BaseCost,
                    MaxPayloadTons = value.MaxPayloadTons,
                    ContractorOverWeightPricePerTon = value.ContractorOverWeightPricePerTon,
                    OverweightCost = value.OverweightCost
                }
            };
        }

        return NotApplicable(contract.ConsumptionType);
    }

    private static PreviewTripContractorPriceResultDto NotApplicable(ContractConsumptionTypes consumptionType) =>
        new()
        {
            ConsumptionType = consumptionType,
            IsApplicable = false,
            FormulaValidationApplies = false,
            ManualEntryRequired = false
        };

    private static decimal RoundExpected(decimal amount) =>
        Math.Round(amount, TripContractorPricingHelper.AmountScale, MidpointRounding.AwayFromZero);

    private static string? ResolvePricingSource(Contract contract, long? contractSlotId)
    {
        if (!TripContractSlotHelper.ShouldDeriveContractSlot(contract) || contractSlotId is not long slotId)
            return "contractVehicleType";

        return contract.ContractSlots.Any(s => s.Id == slotId)
            ? "contractSlot"
            : "contractVehicleType";
    }
}
