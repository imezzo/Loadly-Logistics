using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class UniquenessValidationRules
{
    public static IRuleBuilderOptions<T, string?> IsUnique<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<string, Task<bool>> uniquenessChecker,
        Func<string, string> errorMessageFactory)
    {
        return ruleBuilder
            .MustAsync(async (value, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;
                return await uniquenessChecker(value);
            })
            .WithMessage((context, value) => errorMessageFactory(value ?? string.Empty));
    }

    public static IRuleBuilderOptions<T, string?> IsUnique<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<string, long, Task<bool>> uniquenessChecker,
        Func<T, long> excludeIdSelector,
        Func<string, string> errorMessageFactory)
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;
                var excludeId = excludeIdSelector(instance);
                return await uniquenessChecker(value, excludeId);
            })
            .WithMessage((context, value) => errorMessageFactory(value ?? string.Empty));
    }

    public static IRuleBuilderOptions<T, long> IsUniqueCompositeKey<T>(
        this IRuleBuilder<T, long> ruleBuilder,
        Func<T, long> firstKeySelector,
        Func<T, long> secondKeySelector,
        Func<long, long, Task<bool>> uniquenessChecker,
        Func<string> errorMessageFactory)
    {
        return ruleBuilder
            .MustAsync(async (instance, value, cancellationToken) =>
            {
                var firstKey = firstKeySelector(instance);
                var secondKey = secondKeySelector(instance);
                return await uniquenessChecker(firstKey, secondKey);
            })
            .WithMessage(_ => errorMessageFactory());
    }
}
