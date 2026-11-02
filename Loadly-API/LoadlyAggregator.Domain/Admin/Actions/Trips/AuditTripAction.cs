using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class AuditTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripChargeRepository _tripChargeRepository;
    private readonly IContractRepository _contractRepository;
    private readonly ICurrentUserService _currentUserService;

    public AuditTripAction(
        ITripRepository tripRepository,
        ITripChargeRepository tripChargeRepository,
        IContractRepository contractRepository,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _tripChargeRepository = tripChargeRepository;
        _contractRepository = contractRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(AuditTripDto dto)
    {
        var trip = await _tripRepository.GetByIdForChargeUpdateAsync(dto.Id)
            ?? throw new NotFoundException("trips.not_found_with_id", dto.Id);

        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        if (trip.Status != TripStatus.Completed)
            throw new ValidationException(TripMessageKeys.TripChargesUpdateOnlyWhenCompleted);

        TripValidator.ValidateTripChargesNotInvoiced(trip);
        TripValidator.ValidateTripChargesNotDocumentsApproved(trip);

        if (dto.TripCharges.Count == 0)
            throw new ValidationException(TripMessageKeys.ListCannotBeEmpty);

        await TripCompletedChargeUpsertHelper.UpsertAsync(trip, dto.TripCharges, _tripChargeRepository);

        Contract? contract = trip.ContractId.HasValue
            ? await _contractRepository.GetByIdAsync(trip.ContractId.Value)
            : null;
        TripValidator.ValidateFinancialDate(contract, dto.FinancialDate);
        trip.FinancialDate = dto.FinancialDate;

        TripValidator.ValidateProofOfDeliveryNotTripDerived(trip.Id, dto.ProofOfDelivery);

        trip.ProofOfDelivery = string.IsNullOrWhiteSpace(dto.ProofOfDelivery) ? null : dto.ProofOfDelivery.Trim();

        if (dto.ConfirmFinanceApproval)
        {
            if (string.IsNullOrWhiteSpace(dto.ProofOfDelivery))
                throw new ValidationException(TripMessageKeys.ConfirmFinanceApprovalRequiresProofOfDelivery);

            trip.TripDocumentStatus = TripDocumentStatus.Approved;
        }

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");
        trip.AuditedById = currentUserId;
        trip.AuditedAt = DateTime.UtcNow;

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();
    }
}
