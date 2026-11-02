using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class UpdateTripChargesAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripChargeRepository _tripChargeRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTripChargesAction(
        ITripRepository tripRepository,
        ITripChargeRepository tripChargeRepository,
        IAdminRepository adminRepository,
        IPermissionService permissionService,
        ICurrentUserService currentUserService)
    {
        _tripRepository = tripRepository;
        _tripChargeRepository = tripChargeRepository;
        _adminRepository = adminRepository;
        _permissionService = permissionService;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(UpdateTripChargesDto dto)
    {
        var trip = await _tripRepository.GetByIdForChargeUpdateAsync(dto.Id)
            ?? throw new NotFoundException("trips.not_found_with_id", dto.Id);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.Id);

        var currentUserId = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        if (trip.Status != TripStatus.Completed)
            throw new ValidationException(TripMessageKeys.TripChargesUpdateOnlyWhenCompleted);

        TripValidator.ValidateTripChargesNotInvoiced(trip);
        TripValidator.ValidateTripChargesNotDocumentsApproved(trip);

        if (dto.TripCharges.Count == 0)
            throw new ValidationException(TripMessageKeys.ListCannotBeEmpty);

        await UpsertCompletedTripChargesAsync(trip, dto.TripCharges);

        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();
    }

    private async Task UpsertCompletedTripChargesAsync(Trip trip, IReadOnlyList<UpdateTripChargeItemDto> items)
    {
        List<TripCharge> existingCharges = trip.TripCharges?.ToList() ?? [];
        Dictionary<long, TripCharge> existingById = existingCharges.ToDictionary(c => c.Id);
        HashSet<long> payloadIds = items.Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();

        foreach (TripCharge charge in existingCharges.Where(c => !payloadIds.Contains(c.Id)))
            _tripChargeRepository.Remove(charge);

        List<TripCharge> toAdd = [];
        List<TripCharge> finalCharges = [];

        foreach (UpdateTripChargeItemDto item in items)
        {
            TripChargeType chargeType = (TripChargeType)item.ChargeType;

            if (item.Id > 0)
            {
                if (!existingById.TryGetValue(item.Id, out TripCharge? charge))
                    throw new ValidationException(TripMessageKeys.TripChargeNotFoundOrNotInTrip);
                if (chargeType != charge.ChargeType || item.ExtraFeeTypeId != charge.ExtraFeeTypeId)
                    throw new ValidationException(TripMessageKeys.TripChargeMetadataMismatch);

                charge.Amount = item.Amount;
                charge.CompanyShare = item.CompanyShare;
                finalCharges.Add(charge);
            }
            else
            {
                var charge = new TripCharge
                {
                    TripId = trip.Id,
                    ChargeType = chargeType,
                    ExtraFeeTypeId = item.ExtraFeeTypeId,
                    Amount = item.Amount,
                    CompanyShare = item.CompanyShare
                };
                toAdd.Add(charge);
                finalCharges.Add(charge);
            }
        }

        if (toAdd.Count > 0)
            await _tripChargeRepository.AddRangeAsync(toAdd);

        bool shouldSetOverNightOnCustomer =
            finalCharges.Exists(c => c.ChargeType == TripChargeType.OverNight && c.Amount > 0)
            && trip.OverNightCostBearer is null or OverNightCostBearer.None;
        if (shouldSetOverNightOnCustomer)
            trip.OverNightCostBearer = OverNightCostBearer.Customer;

        RecalculateTripChargeTotals(trip, finalCharges);
    }

    private static void RecalculateTripChargeTotals(Trip trip, IReadOnlyList<TripCharge> charges)
    {
        trip.TotalTripCost = TripChargeTotals.CalculateTotalTripCost(charges);
        trip.TotalAddonsCost = TripChargeTotals.CalculateTotalAddonsCost(charges);
    }
}
