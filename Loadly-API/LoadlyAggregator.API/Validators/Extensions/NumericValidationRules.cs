using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class NumericValidationRules
{
    public static IRuleBuilderOptions<T, int?> IsGreaterThanZero<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, decimal?> IsGreaterThanZero<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, decimal> IsGreaterThanZero<T>(
        this IRuleBuilder<T, decimal> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, decimal?> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }

    public static IRuleBuilderOptions<T, short> IsValidShortRange<T>(
        this IRuleBuilder<T, short> ruleBuilder,
        short min,
        short max,
        string fieldName = "Value")
    {
        return ruleBuilder
            .InclusiveBetween(min, max).WithMessage($"validation.must_be_between|{min}|{max}");
    }

    public static IRuleBuilderOptions<T, int> IsGreaterThanZero<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, short> IsGreaterThanZero<T>(
        this IRuleBuilder<T, short> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan((short)0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, short?> IsGreaterThanZero<T>(
        this IRuleBuilder<T, short?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan((short)0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, decimal> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, decimal> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }

    public static IRuleBuilderOptions<T, int?> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }

    public static IRuleBuilderOptions<T, int> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }

    public static IRuleBuilderOptions<T, int> IsInclusiveRange<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int min,
        int max,
        string fieldName)
    {
        return ruleBuilder
            .InclusiveBetween(min, max).WithMessage($"validation.must_be_between|{min}|{max}");
    }

    public static IRuleBuilderOptions<T, int?> IsInclusiveRange<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        int min,
        int max,
        string fieldName)
    {
        return ruleBuilder
            .InclusiveBetween(min, max).WithMessage($"validation.must_be_between|{min}|{max}");
    }

    public static IRuleBuilderOptions<T, long> IsGreaterThanZero<T>(
        this IRuleBuilderInitial<T, long> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, long> IsGreaterThanZero<T>(
        this IRuleBuilder<T, long> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, long?> IsGreaterThanZero<T>(
        this IRuleBuilderInitial<T, long?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, long?> IsGreaterThanZero<T>(
        this IRuleBuilder<T, long?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, long> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, long> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }

    public static IRuleBuilderOptions<T, long?> IsGreaterThanOrEqualToZero<T>(
        this IRuleBuilder<T, long?> ruleBuilder,
        string fieldName)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }
}
