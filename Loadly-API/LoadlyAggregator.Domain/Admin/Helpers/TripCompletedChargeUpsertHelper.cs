using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public static class TripCompletedChargeUpsertHelper
{
    public static async Task UpsertAsync(
        Trip trip,
        IReadOnlyList<UpdateTripChargeItemDto> items,
        ITripChargeRepository tripChargeRepository)
    {
        List<TripCharge> existingCharges = trip.TripCharges?.ToList() ?? [];
        Dictionary<long, TripCharge> existingById = existingCharges.ToDictionary(c => c.Id);
        HashSet<long> payloadIds = items.Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();

        foreach (TripCharge charge in existingCharges.Where(c => !payloadIds.Contains(c.Id)))
            tripChargeRepository.Remove(charge);

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
            await tripChargeRepository.AddRangeAsync(toAdd);

        RecalculateTripChargeTotals(trip, finalCharges);
    }

    public static void RecalculateTripChargeTotals(Trip trip, IReadOnlyList<TripCharge> charges)
    {
        trip.TotalTripCost = TripChargeTotals.CalculateTotalTripCost(charges);
        trip.TotalAddonsCost = TripChargeTotals.CalculateTotalAddonsCost(charges);
    }
}
