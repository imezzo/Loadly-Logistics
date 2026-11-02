using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Infrastructure.Filters.Implementations;

public class DateRangeFilter<TEntity> : IFilter<TEntity>
{
    private readonly Expression<Func<TEntity, DateTime?>> _datePropertySelector;
    private readonly string _fromKey;
    private readonly string _toKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DateRangeFilter(
        Expression<Func<TEntity, DateTime?>> datePropertySelector,
        IHttpContextAccessor httpContextAccessor,
        string fromKey = "from",
        string toKey = "to")
    {
        _datePropertySelector = datePropertySelector;
        _httpContextAccessor = httpContextAccessor;
        _fromKey = fromKey;
        _toKey = toKey;
        Key = fromKey;
    }

    public string Key { get; }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, object? value)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var visitor = new ParameterReplacer(_datePropertySelector.Parameters[0], parameter);
        var body = visitor.Visit(_datePropertySelector.Body);

        var fromValue = httpContext.Request.Query[_fromKey].ToString();
        if (!string.IsNullOrWhiteSpace(fromValue) && DateTime.TryParse(fromValue, out var fromDate))
        {
            var fromDateUtc = AppUtcTime.UtcStartOfDay(fromDate.Year, fromDate.Month, fromDate.Day);
            var fromConstant = Expression.Constant(fromDateUtc, typeof(DateTime?));
            var fromGreaterThanOrEqual = Expression.GreaterThanOrEqual(body, fromConstant);
            var fromLambda = Expression.Lambda<Func<TEntity, bool>>(fromGreaterThanOrEqual, parameter);
            query = query.Where(fromLambda);
        }

        var toValue = httpContext.Request.Query[_toKey].ToString();
        if (!string.IsNullOrWhiteSpace(toValue) && DateTime.TryParse(toValue, out var toDate))
        {
            var toExclusive = toDate.AddDays(1);
            var toDateUtc = AppUtcTime.UtcStartOfDay(toExclusive.Year, toExclusive.Month, toExclusive.Day);
            var toConstant = Expression.Constant(toDateUtc, typeof(DateTime?));
            var toLessThan = Expression.LessThan(body, toConstant);
            var toLambda = Expression.Lambda<Func<TEntity, bool>>(toLessThan, parameter);
            query = query.Where(toLambda);
        }

        return query;
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
