using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class ExportColumnValidationRules
{
    public static IRuleBuilderOptions<T, List<string>?> AreValidExportColumnKeys<T>(
        this IRuleBuilder<T, List<string>?> ruleBuilder,
        IReadOnlySet<string> validKeys)
    {
        return ruleBuilder
            .Must(columns => columns is not { Count: > 0 } || columns.All(validKeys.Contains))
            .WithMessage("validation.export_column_invalid");
    }
}
