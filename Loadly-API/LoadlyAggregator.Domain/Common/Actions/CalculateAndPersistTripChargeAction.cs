using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Common.TripExtraFeePricing;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Actions;

public class CalculateAndPersistTripChargeAction(
    ITripRepository tripRepository,
    IContractRepository contractRepository,
    ITripChargeRepository tripChargeRepository,
    ITripContractExtraFeePricing contractExtraFeePricing)
{
    private readonly ITripRepository _tripRepository = tripRepository;
    private readonly IContractRepository _contractRepository = contractRepository;
    private readonly ITripChargeRepository _tripChargeRepository = tripChargeRepository;
    private readonly ITripContractExtraFeePricing _contractExtraFeePricing = contractExtraFeePricing;

    public async Task ExecuteAsync(CalculateAndPersistTripChargeDto dto)
    {
        // Contractor payout (Trip.ContractorPrice) is set at trip create/update only; never recalculated here.
        var trip = await _tripRepository.GetByIdAsync(dto.TripId)
            ?? throw new NotFoundException("trips.not_found_with_id", dto.TripId);

        TripValidator.ValidateTripCompletedAndNotAudited(trip);
        TripValidator.ValidateTripChargesNotInvoiced(trip);
        TripValidator.ValidateTripChargesNotDocumentsApproved(trip);

        if (trip.IsExternalTrip)
        {
            await CalculateExternalTripChargesAsync(trip);
            await _tripRepository.SaveChangesAsync();
            return;
        }

        if (!trip.ContractId.HasValue)
            return;

        await CalculateContractTripChargesAsync(trip);
        await _tripRepository.SaveChangesAsync();
    }

    // [External trip] no contract: SpotPrice as base, toll + weight from completion recording as addons.
    private async Task CalculateExternalTripChargesAsync(Trip trip)
    {
        var completeTripAmounts = TripRecordingMetricsBuilder.Build(trip.Recordings);
        var spotPriceCost = trip.SpotPrice ?? 0m;
        var tollGateCost = completeTripAmounts.TollgatesTotalAmount ?? 0m;
        var tollGateWeightCost = completeTripAmounts.TollgatesWeightTotalAmount ?? 0m;

        trip.TotalAddonsCost = tollGateCost + tollGateWeightCost;
        trip.TotalTripCost = spotPriceCost + trip.TotalAddonsCost;
        _tripRepository.Update(trip);

        var charges = new List<TripCharge>
        {
            new() { TripId = trip.Id, ChargeType = TripChargeType.SpotPrice, Amount = spotPriceCost },
            new() { TripId = trip.Id, ChargeType = TripChargeType.TollGate, Amount = tollGateCost, CompanyShare = tollGateCost },
            new() { TripId = trip.Id, ChargeType = TripChargeType.TollGateWeight, Amount = tollGateWeightCost, CompanyShare = tollGateWeightCost }
        };

        await ReplaceTripChargesAsync(trip.Id, charges);
    }

    private async Task CalculateContractTripChargesAsync(Trip trip)
    {
        trip.NumberOfDays = TripContractDestinationHelper.ResolveNumberOfDays(
            trip,
            TripContractDestinationHelper.GetCurrentContractDestination(trip));

        var contract = await _contractRepository.GetByIdOrThrowAsync(trip.ContractId!.Value);

        if (MonthlyContractTripChargePolicy.AppliesTo(contract.ContractType))
        {
            await CalculateMonthlyContractTripChargesAsync(trip, contract);
            return;
        }

        var contractVehicleType = contract.ContractVehicleTypes.FirstOrDefault(cvt => cvt.Id == trip.ContractVehicleTypeId!.Value)
            ?? throw new NotFoundException(
                "contracts.contract_vehicle_type_not_found_on_contract",
                trip.ContractVehicleTypeId!.Value,
                contract.Id);

        var pricing = TripContractPricingHelper.Resolve(contract, trip, contractVehicleType);

        IReadOnlyList<ContractExtraFee> contractExtraFees = contract.ContractExtraFees.ToList();

        var cargoWeightCost = CalculateCargoWeightCharge(
            contract.ConsumptionType,
            trip,
            pricing);
        var cargoCommissionCost = CalculateCargoCommissionCharge(
            contract.ConsumptionType,
            contract.CargoCommission,
            trip);
        var destinationFixedPriceCost = CalculateDestinationFixedPriceCharge(
            contract.ConsumptionType,
            trip);
        var spotPriceCost = CalculateSpotPriceCharge(
            contract.ConsumptionType,
            trip);

        var tripBaseCharge = cargoWeightCost + cargoCommissionCost + destinationFixedPriceCost + spotPriceCost;

        var extraInputs = _contractExtraFeePricing.ForTrip(
            trip,
            tripBaseCharge,
            pricing,
            contract.ReferenceNumber,
            contractExtraFees);

        var charges = new List<TripCharge>();
        AppendContractExtraFeeCharges(charges, trip, trip.Id, contractExtraFees, extraInputs);

        AppendVehicleTypeLineCharges(charges, trip.Id, cargoWeightCost, cargoCommissionCost, destinationFixedPriceCost, spotPriceCost);
        AppendCustomsCharge(charges, trip);
        trip.TotalTripCost = TripChargeTotals.CalculateTotalTripCost(charges);
        trip.TotalAddonsCost = TripChargeTotals.CalculateTotalAddonsCost(charges);
        _tripRepository.Update(trip);

        await ReplaceTripChargesAsync(trip.Id, charges);
    }

    // Included and Excluded monthly: slot calendar carries rental/extra-trip; trip completion is addons only (fuel for Excluded).
    private async Task CalculateMonthlyContractTripChargesAsync(Trip trip, Contract contract)
    {
        var contractVehicleType = contract.ContractVehicleTypes.FirstOrDefault(cvt => cvt.Id == trip.ContractVehicleTypeId!.Value)
            ?? throw new NotFoundException(
                "contracts.contract_vehicle_type_not_found_on_contract",
                trip.ContractVehicleTypeId!.Value,
                contract.Id);

        var pricing = TripContractPricingHelper.Resolve(contract, trip, contractVehicleType);
        IReadOnlyList<ContractExtraFee> contractExtraFees = contract.ContractExtraFees.ToList();
        var addonBase = 0m;

        var extraInputs = _contractExtraFeePricing.ForTrip(
            trip,
            addonBase,
            pricing,
            contract.ReferenceNumber,
            contractExtraFees);

        var charges = new List<TripCharge>();
        AppendContractExtraFeeCharges(charges, trip, trip.Id, contractExtraFees, extraInputs);
        AppendCustomsCharge(charges, trip);
        MonthlyContractTripChargePolicy.ApplyDefaultInvoiceType(charges);

        trip.TotalTripCost = TripChargeTotals.CalculateTotalTripCost(charges);
        trip.TotalAddonsCost = TripChargeTotals.CalculateTotalAddonsCost(charges);
        _tripRepository.Update(trip);

        await ReplaceTripChargesAsync(trip.Id, charges);
    }

    private async Task ReplaceTripChargesAsync(long tripId, IReadOnlyList<TripCharge> charges)
    {
        var existing = (await _tripChargeRepository.FindAsync(c => c.TripId == tripId)).ToList();
        if (existing.Count > 0)
            _tripChargeRepository.RemoveRange(existing);

        if (charges.Count > 0)
            await _tripChargeRepository.AddRangeAsync(charges);
    }

    private void AppendContractExtraFeeCharges(
        List<TripCharge> charges,
        Trip trip,
        long tripId,
        IReadOnlyList<ContractExtraFee> contractExtraFees,
        TripExtraFeeInputs tripInputs)
    {
        var effectiveFees = ContractExtraFeeEffectiveList.WithTollGateWeightFallback(contractExtraFees, tripInputs);

        foreach (var extraFee in effectiveFees)
        {
            if (extraFee.ExtraFeeTypeId == (int)ExtraFeeTypes.Fuel)
            {
                var fuelPrice = _contractExtraFeePricing.CalculateAmount(extraFee, tripInputs);
                if (fuelPrice > 0m)
                {
                    var (fuelAmount, fuelCompanyShare) = FuelExtraFeePolicy.ResolveChargeAmounts(fuelPrice, extraFee.CalculationOption);
                    charges.Add(new TripCharge
                    {
                        TripId = tripId,
                        ChargeType = TripChargeType.Fuel,
                        Amount = fuelAmount,
                        CompanyShare = fuelCompanyShare,
                        ExtraFeeTypeId = extraFee.ExtraFeeTypeId
                    });
                }

                continue;
            }

            if (extraFee.ExtraFeeTypeId == (int)ExtraFeeTypes.OverNight)
            {
                var contractOverNightValue = _contractExtraFeePricing.CalculateAmount(extraFee, tripInputs);
                var (overNightAmount, overNightCompanyShare) = ResolveOverNightChargeAmounts(trip, contractOverNightValue);
                charges.Add(new TripCharge
                {
                    TripId = tripId,
                    ChargeType = TripChargeType.OverNight,
                    Amount = overNightAmount,
                    CompanyShare = overNightCompanyShare,
                    ExtraFeeTypeId = extraFee.ExtraFeeTypeId
                });
                continue;
            }

            var amount = _contractExtraFeePricing.CalculateAmount(extraFee, tripInputs);
            charges.Add(new TripCharge
            {
                TripId = tripId,
                ChargeType = _contractExtraFeePricing.MapToTripChargeType(extraFee.ExtraFeeTypeId),
                Amount = amount,
                ExtraFeeTypeId = extraFee.ExtraFeeTypeId
            });
        }

        AppendOverNightChargeWhenMissingFromContract(charges, trip, tripId, contractExtraFees);
    }

    private static void AppendOverNightChargeWhenMissingFromContract(
        List<TripCharge> charges,
        Trip trip,
        long tripId,
        IReadOnlyList<ContractExtraFee> contractExtraFees)
    {
        if (contractExtraFees.Any(f => f.ExtraFeeTypeId == (int)ExtraFeeTypes.OverNight))
            return;

        if (trip.OverNightCostBearer is not (OverNightCostBearer.Customer or OverNightCostBearer.Loadly))
            return;

        var (amount, companyShare) = ResolveOverNightChargeAmounts(trip, 0m);
        charges.Add(new TripCharge
        {
            TripId = tripId,
            ChargeType = TripChargeType.OverNight,
            Amount = amount,
            CompanyShare = companyShare,
            ExtraFeeTypeId = (int)ExtraFeeTypes.OverNight
        });
    }

    private static (decimal Amount, decimal CompanyShare) ResolveOverNightChargeAmounts(
        Trip trip,
        decimal contractOverNightValue) =>
        trip.OverNightCostBearer switch
        {
            OverNightCostBearer.Loadly => (contractOverNightValue, contractOverNightValue),
            OverNightCostBearer.Customer => (contractOverNightValue, 0m),
            _ => (0m, 0m)
        };

    private static void AppendVehicleTypeLineCharges(
        List<TripCharge> charges,
        long tripId,
        decimal cargoWeightCost,
        decimal cargoCommissionCost,
        decimal destinationFixedPriceCost,
        decimal spotPriceCost)
    {
        // [Base] — one line per vehicle-type structural component, skipped when the amount is 0. Fuel and OverNight are contract extra-fee rows (AppendContractExtraFeeCharges).
        if (cargoWeightCost > 0m)
            charges.Add(new TripCharge { TripId = tripId, ChargeType = TripChargeType.CargoWeight, Amount = cargoWeightCost });
        if (cargoCommissionCost > 0m)
            charges.Add(new TripCharge { TripId = tripId, ChargeType = TripChargeType.CargoCommission, Amount = cargoCommissionCost });
        if (destinationFixedPriceCost > 0m)
            charges.Add(new TripCharge { TripId = tripId, ChargeType = TripChargeType.DestinationFixedPrice, Amount = destinationFixedPriceCost });
        if (spotPriceCost > 0m)
            charges.Add(new TripCharge { TripId = tripId, ChargeType = TripChargeType.SpotPrice, Amount = spotPriceCost });
    }

    private static void AppendCustomsCharge(List<TripCharge> charges, Trip trip)
    {
        if (trip.CustomsStatus != TripCustomsStatus.Cleared)
            return;

        if (trip.CustomsTotalAmount is not > 0m)
            return;

        charges.Add(new TripCharge
        {
            TripId = trip.Id,
            ChargeType = TripChargeType.Customs,
            Amount = trip.CustomsTotalAmount.Value,
            CompanyShare = trip.CustomsTotalAmount.Value
        });
    }

    // Destination fixed-price base charge = the destination's flat per-trip price.
    private static decimal CalculateDestinationFixedPriceCharge(
        ContractConsumptionTypes consumptionType,
        Trip trip)
    {
        if (consumptionType != ContractConsumptionTypes.DESTINATION_FIXED_PRICE)
            return 0m;

        return TripContractDestinationHelper.GetCurrentContractDestination(trip)?.FixedTripPrice ?? 0m;
    }

    // Spot-pricing base charge = the per-trip price negotiated at trip create/update by the dispatcher.
    private static decimal CalculateSpotPriceCharge(
        ContractConsumptionTypes consumptionType,
        Trip trip)
    {
        if (consumptionType != ContractConsumptionTypes.SPOT_PRICING)
            return 0m;

        return trip.SpotPrice ?? 0m;
    }

    // Cargo-weight base charge = billableTons * destination price-per-ton.
    // Overweight (excess beyond MaxPayloadTons) is handled as a contract addon via OverWeightTripExtraFeeCalculator.
    private static decimal CalculateCargoWeightCharge(
        ContractConsumptionTypes consumptionType,
        Trip trip,
        ContractTripPricingTerms pricing)
    {
        if (consumptionType != ContractConsumptionTypes.CARGO_WEIGHTS)
            return 0m;

        var pricePerTon = TripContractDestinationHelper.GetCurrentContractDestination(trip)?.PricePerTon ?? 0m;
        var actualWeight = trip.ActualCargoWeightTons ?? 0m;
        var minBillable = pricing.MinBillableWeightTons ?? 0m;
        var billableTons = Math.Max(actualWeight, minBillable);

        return billableTons * pricePerTon;
    }

    // Cargo-commission base charge = provided cargo price * commission percentage.
    private static decimal CalculateCargoCommissionCharge(
        ContractConsumptionTypes consumptionType,
        decimal cargoCommission,
        Trip trip)
    {
        if (consumptionType != ContractConsumptionTypes.CARGO_PRICE_COMMISSION)
            return 0m;

        var cargoPrice = trip.CargoPrice ?? 0m;

        return (cargoPrice <= 0m || cargoCommission <= 0m) ? 0 : cargoPrice * cargoCommission;
    }

}
