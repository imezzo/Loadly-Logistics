using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class AuthValidationRules
{
    public static IRuleBuilderOptions<T, string> IsValidLogin<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 3)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.login_required")
            .MinimumLength(minLength).WithMessage($"validation.login_min_length|{minLength}");
    }

    public static IRuleBuilderOptions<T, string> IsValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 6)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.password_required")
            .MinimumLength(minLength).WithMessage($"validation.password_min_length_param|{minLength}");
    }

    public static IRuleBuilderOptions<T, string> IsValidNewPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 6)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.new_password_required")
            .MinimumLength(minLength).WithMessage($"validation.password_min_length_param|{minLength}");
    }

    public static IRuleBuilderOptions<T, string> IsValidCurrentPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.current_password_required");
    }

    public static IRuleBuilderOptions<T, string> IsValidConfirmPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.confirmation_password_required");
    }

    public static IRuleBuilderOptions<T, string> IsValidResetToken<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.reset_token_required");
    }

    public static IRuleBuilderOptions<T, string?> IsRequiredToken<T>(this IRuleBuilder<T, string?> ruleBuilder, int minLength = 32)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.token_required")
            .MinimumLength(minLength).WithMessage("validation.token_invalid_length");
    }

    public static IRuleBuilderOptions<T, string?> IsRequiredIdentifier<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.identifier_required_for_reset");
    }

    public static IRuleBuilderOptions<T, string> MustMatchPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        System.Linq.Expressions.Expression<Func<T, string>> passwordSelector,
        string confirmPasswordFieldName = "Confirmation password",
        string passwordFieldName = "Password")
    {
        return ruleBuilder
            .Equal(passwordSelector).WithMessage($"validation.passwords_do_not_match|{confirmPasswordFieldName}|{passwordFieldName}");
    }
}
