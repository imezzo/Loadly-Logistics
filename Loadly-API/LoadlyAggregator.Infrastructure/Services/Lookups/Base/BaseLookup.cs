using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Base;





public abstract class BaseLookup<TEntity> : Lookup
    where TEntity : BaseEntity
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly IHttpContextAccessor HttpContextAccessor;

    protected BaseLookup(
        IRepository<TEntity> repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(languageService)
    {
        Repository = repository;
        HttpContextAccessor = httpContextAccessor;
    }




    protected abstract Expression<Func<TEntity, string>> GetValueSelector();

    protected virtual Expression<Func<TEntity, long>> GetIdSelector() => e => e.Id;




    protected virtual Dictionary<string, Expression<Func<TEntity, object?>>> GetAdditionalFields()
    {
        return new Dictionary<string, Expression<Func<TEntity, object?>>>();
    }

    public override async Task<LookupResponse> ExecuteAsync(LookupRequest request)
    {
        var query = Repository.Query();


        query = ApplyDataSeparation(query);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = ApplyValueSearch(query, request.Search);
        }


        var queryParams = ExtractQueryParameters();
        var utility = new GenericUtility<TEntity>(queryParams);


        query = utility.ApplyOnBuilder(query, request);


        var totalCount = await query.CountAsync();


        var projection = BuildLookupItemProjection();


        var selectedQuery = query.Select(projection);


        var skip = (request.Page - 1) * request.PageSize;
        var items = await selectedQuery
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync();

        return new LookupResponse
        {
            Data = items,
            Pagination = new PaginationDto(request.Page, request.PageSize, totalCount)
        };
    }




    private Expression<Func<TEntity, LookupItem>> BuildLookupItemProjection()
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var lookupItemType = typeof(LookupItem);
        var lookupItemCtor = lookupItemType.GetConstructor(Type.EmptyTypes)!;


        var idSelector = GetIdSelector();
        var idAccess = ReplaceParameter(idSelector.Body, idSelector.Parameters[0], parameter);


        var valueSelector = GetValueSelector();
        var valueAccess = ReplaceParameter(valueSelector.Body, valueSelector.Parameters[0], parameter);


        var memberBindings = new List<MemberBinding>
        {
            Expression.Bind(lookupItemType.GetProperty("Id")!, idAccess),
            Expression.Bind(lookupItemType.GetProperty("Value")!, valueAccess)
        };


        var additionalFields = GetAdditionalFields();
        foreach (var field in additionalFields)
        {
            var lookupItemProperty = lookupItemType.GetProperty(field.Key);
            if (lookupItemProperty != null)
            {
                var fieldExpression = ReplaceParameter(field.Value.Body, field.Value.Parameters[0], parameter);



                Expression convertedExpression;
                var propertyType = lookupItemProperty.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (fieldExpression.Type == propertyType || fieldExpression.Type == underlyingType)
                {
                    convertedExpression = fieldExpression;
                }
                else if (underlyingType == typeof(string))
                {

                    if (fieldExpression.Type == typeof(string))
                    {
                        convertedExpression = fieldExpression;
                    }
                    else
                    {

                        var toStringMethod = fieldExpression.Type.GetMethod("ToString", Type.EmptyTypes);
                        if (toStringMethod != null)
                        {
                            convertedExpression = Expression.Call(fieldExpression, toStringMethod);
                        }
                        else
                        {

                            convertedExpression = Expression.Convert(fieldExpression, typeof(string));
                        }
                    }
                }
                else
                {

                    convertedExpression = Expression.Convert(fieldExpression, propertyType);
                }

                memberBindings.Add(Expression.Bind(lookupItemProperty, convertedExpression));
            }
        }

        var memberInit = Expression.MemberInit(
            Expression.New(lookupItemCtor),
            memberBindings);

        return Expression.Lambda<Func<TEntity, LookupItem>>(memberInit, parameter);
    }




    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParam, ParameterExpression newParam)
    {
        return new ParameterReplacer(oldParam, newParam).Visit(expression) ?? expression;
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




    private Dictionary<string, object?> ExtractQueryParameters()
    {
        var queryParams = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var httpContext = HttpContextAccessor.HttpContext;

        if (httpContext == null)
            return queryParams;

        foreach (var param in httpContext.Request.Query)
        {
            var key = param.Key;
            var value = param.Value.ToString();


            if (key.Equals("page", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("limit", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("search", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("sortBy", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("sortOrder", StringComparison.OrdinalIgnoreCase))
                continue;

            if (string.IsNullOrWhiteSpace(value))
            {
                queryParams[key] = null;
                continue;
            }


            if (long.TryParse(value, out var longValue))
            {
                queryParams[key] = longValue;
            }
            else if (int.TryParse(value, out var intValue))
            {
                queryParams[key] = intValue;
            }
            else
            {
                queryParams[key] = value;
            }
        }

        return queryParams;
    }

    protected virtual IQueryable<TEntity> ApplyValueSearch(IQueryable<TEntity> query, string search)
    {
        var trimmed = search.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return query;
        }

        var valueSelector = GetValueSelector();

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var loweredValue = Expression.Call(valueSelector.Body, toLowerMethod);
        var loweredSearch = Expression.Constant(trimmed.ToLower(), typeof(string));
        var containsCall = Expression.Call(loweredValue, containsMethod, loweredSearch);

        var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, valueSelector.Parameters[0]);
        return query.Where(lambda);
    }

    protected static List<long> ParseIdsFromQuery(IQueryCollection? query, params string[] keys)
    {
        if (query == null || keys.Length == 0)
            return [];

        var ids = new List<long>();
        foreach (var key in keys)
        {
            if (!query.TryGetValue(key, out var values) || values.Count == 0)
                continue;

            ids.AddRange(values
                .SelectMany(v => (v ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Select(v => long.TryParse(v, out var id) ? id : (long?)null)
                .Where(id => id is > 0)
                .Select(id => id!.Value));
        }

        return ids.Distinct().ToList();
    }

    protected virtual IQueryable<TEntity> ApplyDataSeparation(IQueryable<TEntity> query)
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
        {


            return query;
        }




        var userType = httpContext.User?.FindFirst("userType")?.Value;
        if (string.IsNullOrEmpty(userType))
        {


            return query;
        }

        var userIdClaim = httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return query;
        }


        if (string.Equals(userType, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return query;
        }



        return query;
    }
}

