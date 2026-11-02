using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class StringValidationRules
{
    public static IRuleBuilderOptions<T, string?> IsOptionalString<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        string fieldName = "Field")
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}");
    }

    public static IRuleBuilderOptions<T, string?> IsRequiredString<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        string fieldName)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.required")
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}");
    }

    public static IRuleBuilderOptions<T, string?> IsRequiredString<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        int minLength,
        string fieldName)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.required")
            .MinimumLength(minLength).WithMessage($"validation.must_be_at_least_characters|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}");
    }

    public static IRuleBuilderOptions<T, string?> IsOptionalStringWithMaxLength<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength,
        string fieldName)
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}");
    }

    public static IRuleBuilderOptions<T, string?> IsRequiredStringWithMinLength<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength,
        string fieldName)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.required")
            .MinimumLength(minLength).WithMessage($"validation.must_be_at_least_characters|{minLength}");
    }
}
