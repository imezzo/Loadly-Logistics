using System.Linq.Expressions;
using System.Reflection;
using LoadlyAggregator.Shared.DTOs.Common;
namespace LoadlyAggregator.Infrastructure.Services.Lookups.Base;



public class GenericUtility<T>
{
    private readonly Dictionary<string, object?> _queryParams;
    public GenericUtility(Dictionary<string, object?> queryParams)
    {
        _queryParams = queryParams;
    }



    public IQueryable<T> ApplyOnBuilder(IQueryable<T> query, LookupRequest request)
    {

        var typedQuery = query as IQueryable<T>;
        if (typedQuery == null)
        {
            throw new InvalidOperationException($"Cannot cast query to IQueryable<{typeof(T).Name}>");
        }

        typedQuery = ApplyFilters(typedQuery);

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            typedQuery = ApplySorting(typedQuery, request.SortBy, request.SortOrder ?? "asc");
        }
        else
        {

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, "Id");
            var lambda = Expression.Lambda(property, parameter);
            var orderByMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), typeof(long));
            typedQuery = (IQueryable<T>)orderByMethod.Invoke(null, new object[] { typedQuery, lambda })!;
        }
        return typedQuery;
    }
    private IQueryable<T> ApplyFilters(IQueryable<T> query)
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        foreach (var kvp in _queryParams)
        {
            if (kvp.Value == null || string.IsNullOrWhiteSpace(kvp.Value.ToString()))
                continue;
            var property = typeof(T).GetProperty(kvp.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                continue;
            Expression? filterExpression = null;
            if (property.PropertyType == typeof(string))
            {

                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(kvp.Value.ToString(), typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                if (containsMethod != null)
                {
                    filterExpression = Expression.Call(propertyAccess, containsMethod!, constant);
                }
            }
            else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
            {

                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(Convert.ToInt64(kvp.Value), property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {

                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(Convert.ToInt32(kvp.Value), property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {

                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(Convert.ToBoolean(kvp.Value), property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
            else if (TryGetEnumType(property.PropertyType, out var enumType))
            {
                var propertyAccess = Expression.Property(parameter, property);
                var enumValue = Enum.ToObject(enumType, Convert.ToInt32(kvp.Value));
                var constant = Expression.Constant(enumValue, property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }
        }
        return query;
    }

    private static bool TryGetEnumType(Type propertyType, out Type enumType)
    {
        if (propertyType.IsEnum)
        {
            enumType = propertyType;
            return true;
        }

        var underlying = Nullable.GetUnderlyingType(propertyType);
        if (underlying?.IsEnum == true)
        {
            enumType = underlying;
            return true;
        }

        enumType = null!;
        return false;
    }

    private IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy, string sortOrder)
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            return query;
        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);
        var methodName = sortOrder.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        var orderByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType);

        return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, lambda })!;
    }
}
