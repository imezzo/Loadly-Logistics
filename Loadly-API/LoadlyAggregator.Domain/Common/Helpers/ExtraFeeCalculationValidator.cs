using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Validates extra fee type, calculation option, value, and threshold using <see cref="ExtraFeeMetadataRegistry"/> as the single source of rules.
/// </summary>
public static class ExtraFeeCalculationValidator
{
    public static void Validate(long extraFeeTypeId, int? calculationOption, decimal? value, int? threshold)
    {
        _ = Enum.IsDefined(typeof(ExtraFeeTypes), (int)extraFeeTypeId)
            ? true
            : throw new ValidationException("extra_fees.unknown_extra_fee_type_with_id", extraFeeTypeId);
        Validate((ExtraFeeTypes)extraFeeTypeId, calculationOption, value, threshold);
    }

    public static void Validate(ExtraFeeTypes extraFeeType, int? calculationOption, decimal? value, int? threshold)
    {
        var (requiresThreshold, _, typeLevelValueType, _) = ExtraFeeMetadataRegistry.GetThresholdMetadata(extraFeeType);

        _ = ExtraFeeMetadataRegistry.HasNoOptions(extraFeeType)
            ? ValidateMultiStop(calculationOption, requiresThreshold, threshold, typeLevelValueType, value)
            : ValidateWithOptions(extraFeeType, calculationOption, value, threshold, requiresThreshold);
    }

    private static bool ValidateMultiStop(int? calculationOption, bool requiresThreshold, int? threshold, string typeLevelValueType, decimal? value)
    {
        _ = calculationOption == null
            ? true
            : throw new ValidationException("extra_fees.multi_stop_calculation_option_not_null");

        _ = !requiresThreshold || threshold.HasValue
            ? true
            : throw new ValidationException("extra_fees.multi_stop_threshold_required");

        _ = string.IsNullOrEmpty(typeLevelValueType) || value.HasValue
            ? true
            : throw new ValidationException("extra_fees.multi_stop_value_required");

        return true;
    }

    private static bool ValidateWithOptions(ExtraFeeTypes extraFeeType, int? calculationOption, decimal? value, int? threshold, bool requiresThreshold)
    {
        // Special case: Loading/Unloading keeps RequiresThreshold = true in metadata for UI,
        // but the backend accepts a null threshold for this type.
        _ = !requiresThreshold || extraFeeType == ExtraFeeTypes.LoadingUnloading || threshold.HasValue
            ? true
            : throw new ValidationException("extra_fees.threshold_required_for_type", extraFeeType);

        _ = calculationOption.HasValue
            ? true
            : throw new ValidationException("extra_fees.calculation_option_required");

        var optionMeta = ExtraFeeMetadataRegistry.TryGetOptionMeta(extraFeeType, calculationOption.Value);
        _ = optionMeta != null
            ? ValidateOptionValue(optionMeta.Value, value, extraFeeType, calculationOption.Value)
            : throw new ValidationException(
                "extra_fees.invalid_calculation_option_for_type",
                extraFeeType,
                string.Join(", ", ExtraFeeMetadataRegistry.GetValidOptionIds(extraFeeType)));

        return true;
    }

    private static bool ValidateOptionValue((string Label, bool RequiresValue, string? ValueType) optionMeta, decimal? value, ExtraFeeTypes extraFeeType, int calculationOption)
    {
        _ = !optionMeta.RequiresValue || value.HasValue
            ? true
            : throw new ValidationException("extra_fees.value_required_for_option");
        return true;
    }
}
