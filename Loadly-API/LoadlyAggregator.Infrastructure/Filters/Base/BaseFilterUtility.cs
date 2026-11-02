using System.Linq;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Filters.Base;

/// <summary>
/// Base utility class that orchestrates filtering and sorting
/// </summary>
public abstract class BaseFilterUtility<TEntity>
{
    protected readonly IHttpContextAccessor HttpContextAccessor;

    protected BaseFilterUtility(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    protected abstract IEnumerable<IFilter<TEntity>> GetFilters();

    public IQueryable<TEntity> ApplyOnBuilder(IQueryable<TEntity> query, BasePagedRequest? request = null)
    {
        query = ApplyFilters(query);
        query = ApplySorting(query, request);
        return query;
    }

    private IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query)
    {
        var filters = GetFilters();
        var httpContext = HttpContextAccessor.HttpContext;

        if (httpContext == null)
            return query;

        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Key))
                continue;

            var hasValue = httpContext.Request.Query.ContainsKey(filter.Key) ||
                          (httpContext.Request.HasFormContentType && httpContext.Request.Form.ContainsKey(filter.Key));

            if (hasValue)
            {
                object? value = null;
                if (httpContext.Request.Query.ContainsKey(filter.Key))
                {
                    var queryValue = httpContext.Request.Query[filter.Key];
                    if (queryValue.Count == 1)
                        value = queryValue.ToString();
                    else
                        value = queryValue.ToArray();
                }
                else if (httpContext.Request.HasFormContentType && httpContext.Request.Form.ContainsKey(filter.Key))
                {
                    var formValue = httpContext.Request.Form[filter.Key];
                    if (formValue.Count == 1)
                        value = formValue.ToString();
                    else
                        value = formValue.ToArray();
                }

                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    query = filter.Apply(query, value);
                }
            }
        }

        return query;
    }

    private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, BasePagedRequest? request)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
            return ApplyDefaultSorting(query);

        var sortBy = httpContext.Request.Query["sortBy"].ToString();
        var sortOrder = httpContext.Request.Query["sortOrder"].ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            return ApplySortingByProperty(query, sortBy, sortOrder);
        }

        return ApplyDefaultSorting(query);
    }

    private IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
    {
        var createdAtProperty = typeof(TEntity).GetProperty("CreatedAt");
        if (createdAtProperty != null)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
            var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, createdAtProperty);
            var lambda = System.Linq.Expressions.Expression.Lambda(propertyAccess, parameter);
            var orderByMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), typeof(DateTime));
            return (IQueryable<TEntity>)orderByMethod.Invoke(null, new object[] { query, lambda })!;
        }

        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
            var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, idProperty);
            var lambda = System.Linq.Expressions.Expression.Lambda(propertyAccess, parameter);
            var orderByMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), idProperty.PropertyType);
            return (IQueryable<TEntity>)orderByMethod.Invoke(null, new object[] { query, lambda })!;
        }

        return query;
    }

    private IQueryable<TEntity> ApplySortingByProperty(IQueryable<TEntity> query, string sortBy, string sortOrder)
    {
        var property = typeof(TEntity).GetProperty(sortBy,
            System.Reflection.BindingFlags.IgnoreCase |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (property == null)
            return ApplyDefaultSorting(query);

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
        var lambda = System.Linq.Expressions.Expression.Lambda(propertyAccess, parameter);

        var methodName = sortOrder == "desc" ? "OrderByDescending" : "OrderBy";
        var orderByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), property.PropertyType);

        return (IQueryable<TEntity>)orderByMethod.Invoke(null, new object[] { query, lambda })!;
    }

}
