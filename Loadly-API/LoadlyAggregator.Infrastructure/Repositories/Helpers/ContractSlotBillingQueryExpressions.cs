using System.Linq.Expressions;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Infrastructure.Repositories.Helpers;

internal static class ContractSlotBillingQueryExpressions
{
    public static Expression<Func<Trip, bool>> BuildTripMatchesAnySlotExpression(
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots)
    {
        if (slots.Count == 0)
            return _ => false;

        var tripParameter = Expression.Parameter(typeof(Trip), "trip");
        Expression? combined = null;

        foreach (var slot in slots)
        {
            var slotCondition = BuildTripMatchesSlotCondition(tripParameter, slot);
            combined = combined is null
                ? slotCondition
                : Expression.OrElse(combined, slotCondition);
        }

        return Expression.Lambda<Func<Trip, bool>>(combined!, tripParameter);
    }

    private static Expression BuildTripMatchesSlotCondition(
        ParameterExpression tripParameter,
        ContractSlotBillingLoadCriteria slot)
    {
        var vehicleId = Expression.Property(tripParameter, nameof(Trip.VehicleId));
        var contractSlotId = Expression.Property(tripParameter, nameof(Trip.ContractSlotId));

        var pinnedToSlot = Expression.Equal(
            contractSlotId,
            Expression.Constant(slot.SlotId, typeof(long?)));

        Expression slotBinding = pinnedToSlot;
        if (slot.VehicleId is > 0)
        {
            var unassignedWithSeatVehicle = Expression.AndAlso(
                Expression.Equal(contractSlotId, Expression.Constant(null, typeof(long?))),
                Expression.Equal(vehicleId, Expression.Constant(slot.VehicleId, typeof(long?))));
            slotBinding = Expression.OrElse(pinnedToSlot, unassignedWithSeatVehicle);
        }

        var billingOverlap = BuildBillingDaysOverlapSlotCondition(tripParameter, slot);
        var startDateWindow = BuildTripStartUtcWindowCondition(tripParameter, slot);
        var financialDate = Expression.Property(tripParameter, nameof(Trip.FinancialDate));
        var hasFinancialDate = Expression.NotEqual(
            financialDate,
            Expression.Constant(null, typeof(DateOnly?)));

        var dateMatch = Expression.OrElse(
            Expression.AndAlso(hasFinancialDate, billingOverlap),
            Expression.AndAlso(
                Expression.Not(hasFinancialDate),
                startDateWindow));

        return Expression.AndAlso(slotBinding, dateMatch);
    }

    private static Expression BuildBillingDaysOverlapSlotCondition(
        ParameterExpression tripParameter,
        ContractSlotBillingLoadCriteria slot)
    {
        var slotStartDay = EgyptTime.ToCalendarDate(slot.AssignedAtUtc);
        var slotEndDay = slot.ArchivedAtUtc.HasValue
            ? EgyptTime.ToCalendarDate(slot.ArchivedAtUtc.Value)
            : DateOnly.MaxValue;

        var financialDate = Expression.Property(tripParameter, nameof(Trip.FinancialDate));
        var tripStart = Expression.Property(financialDate, nameof(Nullable<DateOnly>.Value));
        var numberOfDays = Expression.Property(tripParameter, nameof(Trip.NumberOfDays));
        var effectiveDays = Expression.Coalesce(numberOfDays, Expression.Constant(1, typeof(int?)));
        var daysToAdd = Expression.Subtract(
            Expression.Convert(effectiveDays, typeof(int)),
            Expression.Constant(1));
        var tripEnd = Expression.Call(
            tripStart,
            nameof(DateOnly.AddDays),
            Type.EmptyTypes,
            daysToAdd);

        var endOnOrAfterSlotStart = Expression.GreaterThanOrEqual(
            tripEnd,
            Expression.Constant(slotStartDay, typeof(DateOnly)));
        var startOnOrBeforeSlotEnd = Expression.LessThanOrEqual(
            tripStart,
            Expression.Constant(slotEndDay, typeof(DateOnly)));

        return Expression.AndAlso(endOnOrAfterSlotStart, startOnOrBeforeSlotEnd);
    }

    private static Expression BuildTripStartUtcWindowCondition(
        ParameterExpression tripParameter,
        ContractSlotBillingLoadCriteria slot)
    {
        var startDate = Expression.Property(tripParameter, nameof(Trip.StartDate));

        var startsOnOrAfterAssignment = Expression.GreaterThanOrEqual(
            startDate,
            Expression.Constant(slot.AssignedAtUtc, typeof(DateTime?)));

        Expression withinActiveWindow = slot.ArchivedAtUtc.HasValue
            ? Expression.LessThan(
                startDate,
                Expression.Constant(slot.ArchivedAtUtc, typeof(DateTime?)))
            : Expression.Constant(true);

        return Expression.AndAlso(startsOnOrAfterAssignment, withinActiveWindow);
    }
}
