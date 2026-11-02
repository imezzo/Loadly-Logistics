using FluentValidation;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class StatusValidationRules
{
    public static IRuleBuilderOptions<T, int> IsValidUserStatus<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .Must(status => Enum.IsDefined(typeof(UserStatus), status))
            .WithMessage($"validation.enum_must_be_one_of|{string.Join(", ", Enum.GetNames(typeof(UserStatus)))}");
    }

    public static IRuleBuilderOptions<T, int> IsValidEnumValue<T, TEnum>(
        this IRuleBuilder<T, int> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => Enum.IsDefined(typeof(TEnum), value))
            .WithMessage($"validation.enum_must_be_one_of|{string.Join(", ", Enum.GetNames(typeof(TEnum)))}");
    }

    public static IRuleBuilderOptions<T, TEnum> IsValidEnumValue<T, TEnum>(
        this IRuleBuilder<T, TEnum> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => Enum.IsDefined(typeof(TEnum), value))
            .WithMessage($"validation.enum_must_be_one_of|{string.Join(", ", Enum.GetNames(typeof(TEnum)))}");
    }

    public static IRuleBuilderOptions<T, int?> IsValidNullableEnumValue<T, TEnum>(
        this IRuleBuilder<T, int?> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => !value.HasValue || Enum.IsDefined(typeof(TEnum), value.Value))
            .WithMessage($"validation.enum_must_be_one_of|{string.Join(", ", Enum.GetNames(typeof(TEnum)))}");
    }

    public static IRuleBuilderOptions<T, int> IsStatusInRange<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int min,
        int max,
        string statusDescription)
    {
        return ruleBuilder
            .InclusiveBetween(min, max).WithMessage($"validation.status_between_with_description|{min}|{max}|{statusDescription}");
    }

    public static IRuleBuilderOptions<T, int?> IsStatusInRange<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        int min,
        int max,
        string statusDescription)
    {
        return ruleBuilder
            .InclusiveBetween(min, max).WithMessage($"validation.status_between_with_description|{min}|{max}|{statusDescription}");
    }

    public static IRuleBuilderOptions<T, TEnum> IsValidEnum<T, TEnum>(
        this IRuleBuilderInitial<T, TEnum> ruleBuilder,
        string fieldName = "Value")
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .IsInEnum().WithMessage($"validation.invalid_named_value|{fieldName}");
    }

    public static IRuleBuilderOptions<T, TEnum> IsValidEnum<T, TEnum>(
        this IRuleBuilder<T, TEnum> ruleBuilder,
        string fieldName = "Value")
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .IsInEnum().WithMessage($"validation.invalid_named_value|{fieldName}");
    }

    public static IRuleBuilderOptions<T, TEnum?> IsValidEnum<T, TEnum>(
        this IRuleBuilderInitial<T, TEnum?> ruleBuilder,
        string fieldName = "Value")
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .IsInEnum().WithMessage($"validation.invalid_named_value|{fieldName}");
    }

    public static IRuleBuilderOptions<T, TEnum?> IsValidEnum<T, TEnum>(
        this IRuleBuilder<T, TEnum?> ruleBuilder,
        string fieldName = "Value")
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .IsInEnum().WithMessage($"validation.invalid_named_value|{fieldName}");
    }

    /// <summary>
    /// Requires a defined enum value (omitted JSON enum binds as 0 when the enum has no zero member).
    /// </summary>
    public static IRuleBuilderOptions<T, TEnum> IsRequiredDefinedEnum<T, TEnum>(
        this IRuleBuilder<T, TEnum> ruleBuilder,
        string requiredMessageKey,
        string invalidMessageKey)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .Must(value => Enum.IsDefined(typeof(TEnum), value))
            .WithMessage((_, value) =>
                Convert.ToInt32(value) == 0 ? requiredMessageKey : invalidMessageKey);
    }
}
