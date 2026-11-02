using FluentValidation;
using LoadlyAggregator.Shared.Utilities;
namespace LoadlyAggregator.API.Validators.Extensions;



public static class ContactValidationRules
{



    public static IRuleBuilderOptions<T, string?> IsRequiredEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 255)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.email_required")
            .EmailAddress().WithMessage("validation.invalid_email_format")
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(email => ValidationHelper.IsValidEmail(email)).WithMessage("validation.invalid_email_format");
    }



    public static IRuleBuilderOptions<T, string?> IsOptionalEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 255)
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(email => string.IsNullOrWhiteSpace(email) || ValidationHelper.IsValidEmail(email))
            .WithMessage("validation.invalid_email_format");
    }



    public static IRuleBuilderOptions<T, string?> IsRequiredEgyptianPhone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 20)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.phone_required")
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(phone => ValidationHelper.IsValidEgyptianPhone(phone)).WithMessage("validation.phone_egyptian_invalid");
    }



    public static IRuleBuilderOptions<T, string?> IsOptionalEgyptianPhone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 20)
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || ValidationHelper.IsValidEgyptianPhone(phone))
            .WithMessage("validation.phone_egyptian_invalid");
    }



    public static IRuleBuilderOptions<T, string?> IsRequiredPhone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 50)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.phone_required")
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(phone => ValidationHelper.IsValidPhone(phone)).WithMessage("validation.phone_invalid");
    }



    public static IRuleBuilderOptions<T, string?> IsOptionalPhone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 50)
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.must_not_exceed_characters|{maxLength}")
            .Must(phone => string.IsNullOrWhiteSpace(phone) || ValidationHelper.IsValidPhone(phone))
            .WithMessage("validation.phone_invalid");
    }
}
