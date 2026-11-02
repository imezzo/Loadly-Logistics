using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;


public static class ExtraFeeMetadataRegistry
{
    public const int MultiStopDefaultIncludedStops = 2;


    private static readonly Dictionary<ExtraFeeTypes, (bool RequiresThreshold, string? ThresholdLabel, string? ThresholdValuePlaceHolder, string? ThresholdDescription)> ThresholdMeta = new()
    {
        // Descriptions are intentionally null for now; you can fill them later per type.
        [ExtraFeeTypes.TollGate] = (false, null, null, null),
        [ExtraFeeTypes.TollGateWeight] = (false, null, null, null),
        [ExtraFeeTypes.OverWeight] = (false, null, null, null),
        [ExtraFeeTypes.HelpersFee] = (true, "Number of Included helpers", "Helpers", "Helpers included in base trip cost at no extra charge"),
        [ExtraFeeTypes.WaitingTime] = (true, "Free waiting period", "Minutes", "No charge for waiting up to this duration"),
        [ExtraFeeTypes.LoadingUnloading] = (true, "Fixed amount per trip", "Minutes free period", "Charged for time spent loading/unloading"),
        [ExtraFeeTypes.MultiStop] = (true, "Stops", "EGP/stop", "Number of Included Stops"),
        [ExtraFeeTypes.IncompleteReturn] = (false, null, null, null),
        [ExtraFeeTypes.Fuel] = (false, null, null, null),
        [ExtraFeeTypes.OverNight] = (false, null, "EGP", "Flat fee in EGP for sleepover on trips under this contract")
    };


    private static readonly Dictionary<ExtraFeeTypes, List<(int OptionEnum, string Label, bool RequiresValue, string? ValueType, string? Description)>> OptionMeta = new()
    {


        [ExtraFeeTypes.Fuel] = new()
        {
            ((int)FuelPricingCalculationOption.IncludedInContractPrice, "Included in contract price", false, null, "Fuel cost is included in contract total price"),
            ((int)FuelPricingCalculationOption.CalculatedPerTrip, "Calculated per trip", false, null, "Fuel cost calculated based on distance and consumption rate per trip")
        },

        [ExtraFeeTypes.TollGate] = new()
        {
            // You can refine Description/Placeholder text later as needed.
            ((int)TollGateCalculation.Included, "Included In Trip Cost", false, null, "Toll gate cost added to each trip invoice"),
            ((int)TollGateCalculation.ClientPay, "Client Pays Directly", false, null, "Client handles toll payments separately"),
            ((int)TollGateCalculation.Fixed, "Fixed amount per trip", true, "EGP", "Same toll cost for all trips regardless of route"),
            ((int)TollGateCalculation.VariablePerRoute, "Variable Per Route", true, "EGP", "Different toll amounts for different routes")
        },
        [ExtraFeeTypes.TollGateWeight] = new()
        {
            ((int)TollGateCalculation.Included, "Included In Trip Cost", false, null, "Toll gate weight cost added to each trip invoice"),
            ((int)TollGateCalculation.ClientPay, "Client Pays Directly", false, null, "Client handles toll payments separately"),
            ((int)TollGateCalculation.Fixed, "Fixed amount per trip", true, "EGP", "Same toll cost for all trips regardless of route"),
            ((int)TollGateCalculation.VariablePerRoute, "Variable Per Route", true, "EGP", "Different toll amounts for different routes")
        },
        [ExtraFeeTypes.OverWeight] = new()
        {
            ((int)WeightFeeCalculation.FullTonOnly, "Full Tons Only", false, null, "Fractional tons are ignored. (Ex: 12.5 = 12 tons charged)"),
            ((int)WeightFeeCalculation.RoundUpPartial, "Partial Tons Included ", false, null, "Fractional tons are rounded up. (Ex: 12.5 = 13 tons charged)"),
            ((int)WeightFeeCalculation.Prorated, "Prorated", false, null, "Charged by exact weight without rounding. (Ex: 12.5 = 12.5 tons charged)"),
        },
        [ExtraFeeTypes.HelpersFee] = new()
        {
            ((int)HelpersFeeCalculation.PerHour, "Per Hour", true, "EGP/hour", "Charge per Helper per Hour"),
            ((int)HelpersFeeCalculation.PerTrip, "Per Trip", true, "EGP/helper/trip", "Flat Rate per Helper per Trip")
        },
        [ExtraFeeTypes.WaitingTime] = new()
        {
            ((int)WaitingTimeFeeCalculation.ProratedMinutes, "Prorated: charge by minutes (e.g 45 min = 0.75 hours)", true, "EGP/hour", null),
            ((int)WaitingTimeFeeCalculation.RoundToHour, "Round up to next hour (45 min = 1 hour charge)", true, "EGP/hour", null),
            ((int)WaitingTimeFeeCalculation.Round15MinBlock, "15-minute increments (45 min = 45 min , 46 min = 60 min) Block", true, "EGP/hour", null),
        },
        [ExtraFeeTypes.LoadingUnloading] = new()
        {
            ((int)LoadingUnloadingCalculation.Included, "Included in Base Rate", false, null, "Loading and unloading included in trip cost"),
            ((int)LoadingUnloadingCalculation.Fixed, "Separate Charge per trip", true, "EGP/trip", "Fixed fee added for loading/unloading"),
            ((int)LoadingUnloadingCalculation.PerItem, "Per Item/Pallet Charge", true, "EGP per Item/Pallet", "Charged based on number of items handled"),
            ((int)LoadingUnloadingCalculation.TimeBased, "Time-based Charge", true, "EGP/hour", "Charged for time spent loading/unloading")
        },
        [ExtraFeeTypes.MultiStop] = new List<(int OptionEnum, string Label, bool RequiresValue, string? ValueType, string? Description)>(),
        [ExtraFeeTypes.OverNight] = new List<(int OptionEnum, string Label, bool RequiresValue, string? ValueType, string? Description)>(),
        [ExtraFeeTypes.IncompleteReturn] = new()
        {
            ((int)IncompleteReturnCalculation.NormalTripCostOnly, "Normal Trip Cost Only", false, null, "Charge: Standard one-way trip cost"),
            ((int)IncompleteReturnCalculation.HalfRound, "Half Round Trip", false, null, "Charge: 50% of what round trip would cost"),
            ((int)IncompleteReturnCalculation.FullRound, "Full Round Trip", false, null, "Charge: Complete round trip cost"),
            ((int)IncompleteReturnCalculation.CustomAmount, "Custom Amount", true, "EGP amount", "For special arrangements")
        }
    };

    /// <summary>Gets threshold metadata for a fee type. Used by validator and builder.</summary>
    public static (bool RequiresThreshold, string? ThresholdLabel, string? ThresholdValuePlaceHolder, string? ThresholdDescription) GetThresholdMetadata(ExtraFeeTypes type)
    {
        return ThresholdMeta.TryGetValue(type, out var m) ? m : (false, null, null, null);
    }

    /// <summary>Gets option metadata for a (type, optionId). Returns null if option is invalid for the type.</summary>
    public static (string Label, bool RequiresValue, string? ValueType)? TryGetOptionMeta(ExtraFeeTypes type, int optionId)
    {
        if (!OptionMeta.TryGetValue(type, out var list))
            return null;
        var row = list.FirstOrDefault(o => o.OptionEnum == optionId);
        return row.Label is null ? null : (row.Label, row.RequiresValue, row.ValueType);
    }

    /// <summary>Returns true if the type has no calculation options (e.g. MultiStop).</summary>
    public static bool HasNoOptions(ExtraFeeTypes type)
    {
        return OptionMeta.TryGetValue(type, out var list) && list.Count == 0;
    }

    /// <summary>Returns valid option IDs for the type (for error messages).</summary>
    public static IReadOnlyList<int> GetValidOptionIds(ExtraFeeTypes type)
    {
        return OptionMeta.TryGetValue(type, out var list) ? list.Select(o => o.OptionEnum).ToList() : Array.Empty<int>();
    }

    /// <summary>Builds the full list of fee type metadata for the API. Uses <see cref="ExtraFeeTypeDisplayNames"/> for names.</summary>
    public static List<ExtraFeeTypeMetaDto> Build()
    {
        var result = new List<ExtraFeeTypeMetaDto>(7);
        foreach (var type in Enum.GetValues<ExtraFeeTypes>())
        {
            var (nameEn, nameAr) = ExtraFeeTypeDisplayNames.Get(type);
            var (requiresThreshold, thresholdLabel, thresholdValuePlaceHolder, thresholdDescription) = GetThresholdMetadata(type);
            var options = OptionMeta.TryGetValue(type, out var list)
                ? list.Select(o => new ExtraFeeOptionMetaDto
                {
                    OptionEnum = o.OptionEnum,
                    Label = o.Label,
                    RequiresValue = o.RequiresValue,
                    ValueType = o.ValueType,
                    Description = o.Description
                }).ToList()
                : new List<ExtraFeeOptionMetaDto>();

            result.Add(new ExtraFeeTypeMetaDto
            {
                TypeId = (int)type,
                NameEn = nameEn,
                NameAr = nameAr,
                RequiresThreshold = requiresThreshold,
                ThresholdLabel = thresholdLabel,
                ThresholdValuePlaceHolder = thresholdValuePlaceHolder,
                ThresholdDescription = thresholdDescription,
                Options = options,
            });
        }
        return result;
    }
}
