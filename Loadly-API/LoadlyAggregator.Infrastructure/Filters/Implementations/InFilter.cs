using System.Linq;
using System.Linq.Expressions;

namespace LoadlyAggregator.Infrastructure.Filters.Implementations;

public class InFilter<TEntity, TValue> : IFilter<TEntity>
    where TValue : struct
{
    private readonly Expression<Func<TEntity, TValue?>> _propertySelector;

    public InFilter(string key, Expression<Func<TEntity, TValue?>> propertySelector)
    {
        Key = key;
        _propertySelector = propertySelector;
    }

    public string Key { get; }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, object? value)
    {
        if (value == null)
            return query;

        var values = ParseValues(value);
        if (values == null || !values.Any())
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var visitor = new ParameterReplacer(_propertySelector.Parameters[0], parameter);
        var body = visitor.Visit(_propertySelector.Body);

        var containsMethod = typeof(ICollection<TValue>).GetMethod("Contains", new[] { typeof(TValue) });
        if (containsMethod == null)
            return query;

        var constant = Expression.Constant(values, typeof(ICollection<TValue>));
        var containsCall = Expression.Call(constant, containsMethod, body);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, parameter);

        return query.Where(lambda);
    }

    private ICollection<TValue>? ParseValues(object value)
    {
        var values = new List<TValue>();

        if (value is IEnumerable<object> enumerable)
        {
            foreach (var item in enumerable)
            {
                if (TryParseSingleValue(item, out var parsed))
                    values.Add(parsed);
            }
        }
        else if (value is string stringValue)
        {
            var parts = stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (TryParseSingleValue(part.Trim(), out var parsed))
                    values.Add(parsed);
            }
        }
        else
        {
            if (TryParseSingleValue(value, out var parsed))
                values.Add(parsed);
        }

        return values.Any() ? values : null;
    }

    private bool TryParseSingleValue(object value, out TValue parsed)
    {
        parsed = default;

        if (value is TValue directValue)
        {
            parsed = directValue;
            return true;
        }

        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
            return false;

        if (typeof(TValue) == typeof(long) || typeof(TValue) == typeof(long?))
        {
            if (long.TryParse(stringValue, out var longVal))
            {
                parsed = (TValue)(object)longVal;
                return true;
            }
        }
        else if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
        {
            if (int.TryParse(stringValue, out var intVal))
            {
                parsed = (TValue)(object)intVal;
                return true;
            }
        }

        return false;
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly ParameterExpression _newParam;

        public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
        {
            _oldParam = oldParam;
            _newParam = newParam;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }
}
