using FluentValidation;
namespace LoadlyAggregator.API.Validators.Extensions;



public static class MultilingualValidationRules
{



    public static IRuleBuilderOptions<T, string?> IsRequiredEnglishName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 255)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.english_name_required")
            .MinimumLength(minLength).WithMessage($"validation.english_name_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.english_name_max_length|{maxLength}");
    }



    public static IRuleBuilderOptions<T, string?> IsRequiredArabicName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 255)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.arabic_name_required")
            .MinimumLength(minLength).WithMessage($"validation.arabic_name_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.arabic_name_max_length|{maxLength}");
    }



    public static IRuleBuilderOptions<T, string?> IsOptionalArabicName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength = 255)
    {
        return ruleBuilder
            .MaximumLength(maxLength).WithMessage($"validation.arabic_name_max_length|{maxLength}");
    }



    public static IRuleBuilderOptions<T, string?> IsRequiredFullName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 200)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.full_name_required")
            .MinimumLength(minLength).WithMessage($"validation.full_name_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.full_name_max_length|{maxLength}");
    }



    public static IRuleBuilderOptions<T, string?> IsOptionalFullName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 200)
    {
        return ruleBuilder
            .MinimumLength(minLength).WithMessage($"validation.full_name_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.full_name_max_length|{maxLength}");
    }



    public static IRuleBuilderOptions<T, string?> IsRequiredEnglishFullName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 255)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.full_name_english_required")
            .MinimumLength(minLength).WithMessage($"validation.full_name_english_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.full_name_english_max_length|{maxLength}")
            .Matches(@"^[A-Za-z\s]+$")
            .WithMessage("validation.full_name_english_letters_only");
    }


    public static IRuleBuilderOptions<T, string?> IsRequiredArabicFullName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int minLength = 2,
        int maxLength = 255)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.full_name_arabic_required")
            .MinimumLength(minLength).WithMessage($"validation.full_name_arabic_min_length|{minLength}")
            .MaximumLength(maxLength).WithMessage($"validation.full_name_arabic_max_length|{maxLength}")
            .Matches(@"^[\u0600-\u06FF\s]+$")
            .WithMessage("validation.full_name_arabic_letters_only");
    }

}
