using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class IdValidationRules
{
    public static IRuleBuilderOptions<T, long> IsRequiredId<T>(
        this IRuleBuilder<T, long> ruleBuilder,
        string entityName = "ID")
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage($"validation.entity_must_be_greater_than_zero|{entityName}");
    }

    public static IRuleBuilderOptions<T, long?> IsOptionalId<T>(
        this IRuleBuilder<T, long?> ruleBuilder,
        string entityName = "ID")
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage($"validation.entity_must_be_greater_than_zero|{entityName}");
    }
}
