using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;






public class AdminsDefaultLookup : BaseLookup<Admin>
{
    public AdminsDefaultLookup(
        IAdminRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Admin, string>> GetValueSelector()
        => x => x.FullName;

    protected override Dictionary<string, Expression<Func<Admin, object?>>> GetAdditionalFields()
        => new()
        {
            { "Phone", x => x.Phone },
            { "Email", x => x.Email }
        };

    protected override IQueryable<Admin> ApplyDataSeparation(IQueryable<Admin> query)
    {
        var adminIds = ParseIdsFromQuery(HttpContextAccessor.HttpContext?.Request.Query, "adminIds");
        return adminIds.Count > 0 ? query.Where(a => adminIds.Contains(a.Id)) : query;
    }
}

