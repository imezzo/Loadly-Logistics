using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class CompaniesDefaultLookup : BaseLookup<Company>
{
    public CompaniesDefaultLookup(
        ICompanyRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Company, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? c => c.CompanyDetail != null
                ? (c.CompanyDetail.NameAr ?? c.CompanyDetail.NameEn ?? string.Empty)
                : string.Empty
            : c => c.CompanyDetail != null
                ? (c.CompanyDetail.NameEn ?? c.CompanyDetail.NameAr ?? string.Empty)
                : string.Empty;
    }

    protected override IQueryable<Company> ApplyDataSeparation(IQueryable<Company> query)
    {
        return query.Where(c => c.CompanyId == null);
    }
}

