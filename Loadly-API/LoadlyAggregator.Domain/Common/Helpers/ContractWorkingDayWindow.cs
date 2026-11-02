using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

public readonly record struct ContractWorkingDayWindow(TimeOnly Start, TimeOnly End)
{
    /// <summary>Full Egypt calendar day, midnight to the next midnight.</summary>
    public static ContractWorkingDayWindow Default { get; } = new(TimeOnly.MinValue, TimeOnly.MinValue);

    /// <summary>
    /// Returns the half-open UTC instant range <c>[start, end)</c> for the working day labeled by <paramref name="date"/>
    /// in the business time zone (<see cref="EgyptTime"/>). When <see cref="End"/> is on or before <see cref="Start"/>,
    /// the window spans into the next calendar day.
    /// </summary>
    public (DateTime Start, DateTime End) ToUtcRange(DateOnly date) =>
        EgyptTime.ToUtcHalfOpenRange(date, Start, End);
}
