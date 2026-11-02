using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public static class ContractFuelExtraFeeHelper
{
    public static List<ExtraFeeItemDto> AddFuelToContractExtraFees(
        List<ExtraFeeItemDto>? extraFees,
        ContractType contractType,
        ContractConsumptionTypes consumptionType)
    {
        var list = extraFees ?? new List<ExtraFeeItemDto>();
        long fuelTypeId = (long)ExtraFeeTypes.Fuel;
        list.RemoveAll(e => e.ExtraFeeTypeId == fuelTypeId);

        list.Add(new ExtraFeeItemDto
        {
            ExtraFeeTypeId = fuelTypeId,
            CalculationOption = (int)ResolveFuelPricingCalculationOption(contractType, consumptionType),
            Value = null,
            Threshold = null
        });
        return list;
    }

    public static FuelPricingCalculationOption ResolveFuelPricingCalculationOption(
        ContractType contractType,
        ContractConsumptionTypes consumptionType) =>
        IsContractTripPricingConveringFuel(contractType, consumptionType)
            ? FuelPricingCalculationOption.IncludedInContractPrice
            : FuelPricingCalculationOption.CalculatedPerTrip;

    public static FuelPricingCalculationOption ResolveFuelPricingCalculationOption(Contract? contract) =>
        contract is null
            ? FuelPricingCalculationOption.IncludedInContractPrice
            : ResolveFuelPricingCalculationOption(contract.ContractType, contract.ConsumptionType);

    public static bool IsContractTripPricingConveringFuel(ContractType contractType, ContractConsumptionTypes consumptionType)
    {
        if (contractType == ContractType.Included)
            return true;
        if (contractType == ContractType.OnDemand && consumptionType != ContractConsumptionTypes.KMs)
            return true;
        return false;
    }

}
