using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class PushTripToContractorsAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _contractDestinationRepository;
    private readonly IContractVehicleTypeRepository _contractVehicleTypeRepository;
    private readonly PushTripToContractorHelper _pushTripToContractorHelper;

    public PushTripToContractorsAction(
        ITripRepository tripRepository,
        IContractRepository contractRepository,
        IContractDestinationRepository contractDestinationRepository,
        IContractVehicleTypeRepository contractVehicleTypeRepository,
        PushTripToContractorHelper pushTripToContractorHelper)
    {
        _tripRepository = tripRepository;
        _contractRepository = contractRepository;
        _contractDestinationRepository = contractDestinationRepository;
        _contractVehicleTypeRepository = contractVehicleTypeRepository;
        _pushTripToContractorHelper = pushTripToContractorHelper;
    }

    public async Task ExecuteAsync(PushTripToContractorsDto dto)
    {
        var tripId = dto.TripId;

        var trip = await _tripRepository.GetByIdOrThrowAsync(tripId);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", tripId);

        TripValidator.ValidateTripNotAuditedOrInvoiced(trip);
        TripValidator.ValidateDispatchingChange(trip, TripDispatchingMode.Contractors);
        TripValidator.ValidateContractRequired(trip.IsExternalTrip, trip.ContractId);

        if (!trip.ContractId.HasValue || trip.ContractId.Value <= 0)
            throw new ValidationException(TripMessageKeys.ContractRequiredForContractScopedFields);

        var contractId = trip.ContractId.Value;
        var contract = trip.Contract
            ?? await _contractRepository.GetByIdAsync(contractId)
            ?? throw new NotFoundException("contracts.not_found_with_id", contractId);

        TripValidator.ValidateContractorsDispatchAllowed(contract, pushToContractors: true, isExternalTrip: trip.IsExternalTrip);

        var contractVehicleType = await _contractVehicleTypeRepository.GetByContractIdAndVehicleTypeIdAsync(
                contractId,
                dto.VehicleTypeId)
            ?? throw new NotFoundException("trips.contract_vehicle_type_not_found_for_vehicle_type", dto.VehicleTypeId);

        var tripDestination = await TripContractDestinationHelper.GetOptionalContractDestinationForTripAsync(
            contractId,
            TripContractDestinationHelper.GetCurrentContractDestinationId(trip),
            _contractDestinationRepository);

        var contractSlotId = TripContractSlotHelper.ResolveContractSlotIdForTrip(
            contract,
            trip.VehicleId,
            tripStartUtc: trip.StartDate);

        TripValidator.ValidateSpotPrice(contract, dto.SpotPrice);
        TripValidator.ValidateSubmittedContractorPrice(
            contract,
            trip.Status,
            dto.ContractorPrice,
            trip.ActualCargoWeightTons,
            contractVehicleType,
            tripDestination,
            contractSlotId);

        trip.ContractVehicleTypeId = contractVehicleType.Id;
        trip.SpotPrice = dto.SpotPrice;
        trip.ContractorPrice = dto.ContractorPrice;
        trip.DispatchingMode = TripDispatchingMode.Contractors;

        TripValidator.ValidateContractorAssign(TripDispatchingMode.Contractors, null, trip);

        if (dto.SendToAllContractors)
            TripValidator.ValidatePushToAllContractors(trip);

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        await _pushTripToContractorHelper.InviteContractorsAsync(
            trip,
            dto.SendToAllContractors,
            dto.ContractorIds);
    }
}
