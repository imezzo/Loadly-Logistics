using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;





public class IndustryTypesDefaultLookup : BaseLookup<IndustryType>
{
    public IndustryTypesDefaultLookup(
        IIndustryTypeRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<IndustryType, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }
}

