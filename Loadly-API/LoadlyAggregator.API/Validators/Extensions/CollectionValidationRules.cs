using System.Linq;
using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class CollectionValidationRules
{
    public static IRuleBuilderOptions<T, TCollection> IsRequiredCollection<T, TCollection>(
        this IRuleBuilder<T, TCollection> ruleBuilder,
        string fieldName = "Collection")
        where TCollection : System.Collections.IEnumerable
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"validation.collection_cannot_be_empty|{fieldName}")
            .Must(collection => collection != null && collection.Cast<object>().Any())
            .WithMessage($"validation.collection_at_least_one|{fieldName}");
    }

    public static IRuleBuilderOptions<T, TCollection> IsRequiredNonEmptyCollection<T, TCollection>(
        this IRuleBuilder<T, TCollection> ruleBuilder,
        string fieldName = "Collection")
        where TCollection : System.Collections.IEnumerable
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"validation.collection_cannot_be_empty|{fieldName}")
            .Must(collection => collection != null && collection.Cast<object>().Any())
            .WithMessage($"validation.collection_at_least_one|{fieldName}");
    }
}
