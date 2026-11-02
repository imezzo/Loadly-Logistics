using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class PaginationValidationRules
{
    public static IRuleBuilderOptions<T, int> IsValidPage<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.page_must_be_greater_than_zero");
    }

    public static IRuleBuilderOptions<T, int> IsValidPageSize<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int maxSize = 100)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("validation.page_size_must_be_greater_than_zero")
            .LessThanOrEqualTo(maxSize).WithMessage($"validation.page_size_must_not_exceed|{maxSize}");
    }
}
