using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class CalculateContractDestinationCostsAction
{
    private const long DefaultFuelTypeId = 1;

    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly IFuelTypeRepository _fuelTypeRepository;

    public CalculateContractDestinationCostsAction(
        IVehicleTypeRepository vehicleTypeRepository,
        IFuelTypeRepository fuelTypeRepository)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
        _fuelTypeRepository = fuelTypeRepository;
    }

    public async Task<CalculateContractDestinationCostsDto> ExecuteAsync(CalculateContractDestinationCostsRequestDto dto)
    {
        var response = new CalculateContractDestinationCostsDto();

        foreach (var vehicleTypeInput in dto.VehicleTypes)
        {
            var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(vehicleTypeInput.VehicleTypeId);
            decimal fuelUnitPrice = await ResolveFuelUnitPriceAsync();

            var baseDailyValue = ContractCalculationHelper.CalculateDailyValue(
                vehicleTypeInput.MonthlyRental,
                ContractCalculationHelper.ResolveMonthlyWorkingDays(dto.MonthlyWorkingDays, dto.ContractType),
                dto.ContractType) ?? 0m;

            var vehicleTypeResult = new CalculatedVehicleTypeCostDto
            {
                VehicleTypeId = vehicleType.Id,
                VehicleTypeNameAr = vehicleType.NameAr,
                VehicleTypeNameEn = vehicleType.NameEn
            };

            foreach (var destination in dto.Destinations)
            {
                int numberOfDays = destination.NumberOfDays ?? 1; // Default to 1 day if not provided
                decimal kms = destination.Kms ?? 0m;
                decimal calculatedDailyValue = baseDailyValue * numberOfDays;
                decimal fuelPrice = dto.ContractType == ContractType.Included
                    ? 0m
                    : (kms / vehicleTypeInput.ConsumptionRate) * fuelUnitPrice;

                var dailyValueRounded = RoundToTwoDecimals(calculatedDailyValue);
                var fuelPriceRounded = RoundToTwoDecimals(fuelPrice);

                vehicleTypeResult.Destinations.Add(new CalculatedDestinationCostDto
                {
                    Kms = destination.Kms,
                    NumberOfDays = numberOfDays,
                    NameAr = destination.NameAr,
                    NameEn = destination.NameEn,
                    DailyValue = dailyValueRounded,
                    FuelPrice = fuelPriceRounded,
                    TotalTripCost = dailyValueRounded + fuelPriceRounded
                });
            }

            response.VehicleTypes.Add(vehicleTypeResult);
        }

        return response;
    }

    private async Task<decimal> ResolveFuelUnitPriceAsync()
    {
        var fuelType = await _fuelTypeRepository.GetByIdOrThrowAsync(DefaultFuelTypeId);
        if (fuelType.UnitPrice <= 0)
            throw new ValidationException("fuel_types.must_have_positive_unit_price", DefaultFuelTypeId);
        return fuelType.UnitPrice;
    }

    private static decimal RoundToTwoDecimals(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
