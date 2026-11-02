using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class CollectorsDefaultLookup : BaseLookup<Collector>
{
    public CollectorsDefaultLookup(
        ICollectorRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Collector, string>> GetValueSelector()
        => x => x.FullName;

    protected override Dictionary<string, Expression<Func<Collector, object?>>> GetAdditionalFields()
        => new()
        {
            { "Phone", x => x.Phone },
            { "Email", x => x.Email }
        };

    protected override IQueryable<Collector> ApplyDataSeparation(IQueryable<Collector> query)
    {
        return query;
    }
}
