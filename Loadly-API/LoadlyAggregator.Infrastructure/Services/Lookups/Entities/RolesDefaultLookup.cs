using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class RolesDefaultLookup : BaseLookup<Role>
{
    private const string CompanyUserType = "company";
    private const string CompanyGuard = "company";

    public RolesDefaultLookup(
        IRoleRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Role, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.NameAr ?? x.Name
            : x => x.Name;
    }

    protected override IQueryable<Role> ApplyDataSeparation(IQueryable<Role> query)
    {
        var userType = HttpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value;

        if (string.Equals(userType, CompanyUserType, StringComparison.OrdinalIgnoreCase))
        {
            if (!long.TryParse(HttpContextAccessor.HttpContext?.User?.FindFirst("companyId")?.Value, out var companyId)
                || companyId == 0)
            {
                return query.Where(_ => false);
            }

            return query.Where(r => r.GuardName == CompanyGuard && companyId == r.CompanyId);
        }

        query = query.Where(r => r.CompanyId == null);

        var guardName = HttpContextAccessor.HttpContext?.Request.Query["guardName"].ToString();
        return string.IsNullOrWhiteSpace(guardName)
            ? query.Where(_ => false)
            : query.Where(r => r.GuardName.ToLower() == guardName.Trim().ToLowerInvariant());
    }
}
