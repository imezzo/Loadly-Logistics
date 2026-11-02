using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class DriverValidationRules
{
    public static IRuleBuilderOptions<T, string?> IsRequiredNationalId<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string fieldName = "National ID")
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"validation.field_is_required|{fieldName}")
            .Matches(@"^\d{14}$").WithMessage($"validation.national_id_must_be_14_digits|{fieldName}");
    }
}
