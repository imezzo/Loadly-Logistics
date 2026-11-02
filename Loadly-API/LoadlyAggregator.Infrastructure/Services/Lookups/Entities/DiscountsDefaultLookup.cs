using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class DiscountsDefaultLookup : BaseLookup<Discount>
{
    public DiscountsDefaultLookup(
        IDiscountRepository repository,
        Domain.Common.Interfaces.ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Discount, string>> GetValueSelector()
    {
        // Frontend expects LookupItem.Value as string; discounts table stores numeric Value.
        return x => x.Value.ToString();
    }
}

