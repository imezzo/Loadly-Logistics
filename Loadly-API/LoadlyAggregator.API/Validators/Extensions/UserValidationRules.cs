using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class UserValidationRules
{
    public static IRuleBuilderOptions<T, string> IsRequiredUserName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 3,
        int maxLength = 50)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.username_required")
            .MinimumLength(minLength).WithMessage($"validation.username_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.username_max_length|{maxLength}")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern");
    }

    public static IRuleBuilderOptions<T, string> IsOptionalUserName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 3,
        int maxLength = 50)
    {
        return ruleBuilder
            .MinimumLength(minLength).WithMessage($"validation.username_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.username_max_length|{maxLength}")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern");
    }
}
