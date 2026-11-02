using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class PortsDefaultLookup : BaseLookup<Port>
{
    public PortsDefaultLookup(
        IPortRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Port, string>> GetValueSelector()
    {
        string language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }
}
