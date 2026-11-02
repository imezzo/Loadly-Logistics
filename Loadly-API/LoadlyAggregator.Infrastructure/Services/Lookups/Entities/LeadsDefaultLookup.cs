using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class LeadsDefaultLookup : BaseLookup<Lead>
{
    public LeadsDefaultLookup(
        ILeadRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Lead, string>> GetValueSelector() =>
        x => (x.FullName ?? string.Empty) + " - " + (x.Phone ?? string.Empty);
}
