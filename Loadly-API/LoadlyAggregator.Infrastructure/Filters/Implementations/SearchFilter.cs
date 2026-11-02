using System.Linq;
using System.Linq.Expressions;

namespace LoadlyAggregator.Infrastructure.Filters.Implementations;

public class SearchFilter<TEntity> : IFilter<TEntity>
{
    private readonly Expression<Func<TEntity, string>>[] _searchProperties;

    public SearchFilter(params Expression<Func<TEntity, string>>[] searchProperties)
    {
        _searchProperties = searchProperties;
        Key = "search";
    }

    public string Key { get; }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return query;

        var searchTerm = value.ToString()!.ToLower();

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? combinedExpression = null;

        foreach (var searchProperty in _searchProperties)
        {
            var visitor = new ParameterReplacer(searchProperty.Parameters[0], parameter);
            var body = visitor.Visit(searchProperty.Body);

            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var lowerBody = Expression.Call(body, toLowerMethod!);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var constant = Expression.Constant(searchTerm, typeof(string));
            var containsCall = Expression.Call(lowerBody, containsMethod!, constant);

            var nullCheck = Expression.NotEqual(body, Expression.Constant(null, typeof(string)));
            var nullSafeContains = Expression.AndAlso(nullCheck, containsCall);

            combinedExpression = combinedExpression == null
                ? nullSafeContains
                : Expression.OrElse(combinedExpression, nullSafeContains);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
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
