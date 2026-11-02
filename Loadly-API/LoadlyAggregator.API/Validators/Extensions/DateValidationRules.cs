using System.Linq.Expressions;
using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

/// <summary>
/// FluentValidation date rules. Thresholds use <b>UTC</b> (<see cref="TimeProvider.System"/>) so they align with API instants
/// and PostgreSQL <c>timestamptz</c>. Calendar-day rules compare <see cref="DateOnly"/> built from year/month/day of the value
/// against today’s calendar date in UTC (no business-region timezone).
/// </summary>
public static class DateValidationRules
{
    private static DateTime UtcNow() => TimeProvider.System.GetUtcNow().UtcDateTime;

    private static DateOnly UtcTodayCalendar() => DateOnly.FromDateTime(UtcNow());

    /// <summary>Calendar date from clock components only (ignores <see cref="DateTime.Kind"/>).</summary>
    private static DateOnly CalendarDate(DateTime value) => new(value.Year, value.Month, value.Day);

    public static IRuleBuilderOptions<T, DateTime> IsRequiredDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"validation.field_is_required|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsRequiredDate<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"validation.field_is_required|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsAfterStartDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        Expression<Func<T, DateTime>> startDateSelector,
        string endDateFieldName = "End date",
        string startDateFieldName = "Start date")
    {
        return ruleBuilder
            .GreaterThan(startDateSelector).WithMessage($"validation.field_must_be_after|{endDateFieldName}|{startDateFieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsAfterDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        Expression<Func<T, DateTime>> otherDateSelector,
        string fieldName = "End date",
        string otherFieldName = "Start date")
    {
        return ruleBuilder
            .GreaterThan(otherDateSelector).WithMessage($"validation.field_must_be_after|{fieldName}|{otherFieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsFutureDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => value > UtcNow())
            .WithMessage($"validation.field_must_be_in_future|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsFutureDate<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => !value.HasValue || value.Value > UtcNow())
            .WithMessage($"validation.field_must_be_in_future|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsPastOrPresent<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => value <= UtcNow())
            .WithMessage($"validation.field_must_not_be_in_future|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsPastOrPresent<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => !value.HasValue || value.Value <= UtcNow())
            .WithMessage($"validation.field_must_not_be_in_future|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsGreaterThanOrEqualToDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        Expression<Func<T, DateTime>> otherDateSelector,
        string fieldName = "End date",
        string otherFieldName = "Start date")
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(otherDateSelector)
            .WithMessage($"validation.field_must_be_after_or_equal|{fieldName}|{otherFieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsGreaterThanOrEqualToDate<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        Expression<Func<T, DateTime?>> otherDateSelector,
        string fieldName = "End date",
        string otherFieldName = "Start date")
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(otherDateSelector)
            .WithMessage($"validation.field_must_be_after_or_equal|{fieldName}|{otherFieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsRequiredFutureDate<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .NotNull().WithMessage($"validation.field_is_required|{fieldName}")
            .Must(value => value.HasValue && value.Value > UtcNow())
            .WithMessage($"validation.field_must_be_in_future|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsPastDateTime<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => value < UtcNow())
            .WithMessage($"validation.field_must_be_in_past|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime?> IsPastDate<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => !value.HasValue || CalendarDate(value.Value) < UtcTodayCalendar())
            .WithMessage($"validation.field_must_be_in_past|{fieldName}");
    }

    public static IRuleBuilderOptions<T, DateTime> IsOnOrBeforeTodayDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        string fieldName = "Date")
    {
        return ruleBuilder
            .Must(value => CalendarDate(value) <= UtcTodayCalendar())
            .WithMessage($"validation.field_today_or_past|{fieldName}");
    }
}
