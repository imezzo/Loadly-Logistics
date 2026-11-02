using System.Linq;
using System.Linq.Expressions;

namespace LoadlyAggregator.Infrastructure.Filters.Implementations;

public class EqualityFilter<TEntity, TValue> : IFilter<TEntity>
    where TValue : struct
{
    private readonly Expression<Func<TEntity, TValue?>> _propertySelector;

    public EqualityFilter(string key, Expression<Func<TEntity, TValue?>> propertySelector)
    {
        Key = key;
        _propertySelector = propertySelector;
    }

    public string Key { get; }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return query;

        if (!TryParseValue(value, out var parsedValue))
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var visitor = new ParameterReplacer(_propertySelector.Parameters[0], parameter);
        var body = visitor.Visit(_propertySelector.Body);

        var constant = Expression.Constant(parsedValue, typeof(TValue?));
        var equalExpression = Expression.Equal(body, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);

        return query.Where(lambda);
    }

    private bool TryParseValue(object value, out TValue? parsedValue)
    {
        parsedValue = null;

        if (value is TValue directValue)
        {
            parsedValue = directValue;
            return true;
        }

        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
            return false;

        if (typeof(TValue) == typeof(long) || typeof(TValue) == typeof(long?))
        {
            if (long.TryParse(stringValue, out var longVal))
            {
                parsedValue = (TValue)(object)longVal;
                return true;
            }
        }
        else if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
        {
            if (int.TryParse(stringValue, out var intVal))
            {
                parsedValue = (TValue)(object)intVal;
                return true;
            }
        }
        else if (typeof(TValue) == typeof(bool) || typeof(TValue) == typeof(bool?))
        {
            if (bool.TryParse(stringValue, out var boolVal))
            {
                parsedValue = (TValue)(object)boolVal;
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
