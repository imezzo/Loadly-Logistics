using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class ContractorsDefaultLookup : BaseLookup<Contractor>
{
    public ContractorsDefaultLookup(
        IContractorRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Contractor, string>> GetValueSelector()
        => x => x.FullName;

    protected override Dictionary<string, Expression<Func<Contractor, object?>>> GetAdditionalFields()
        => new()
        {
            { "Phone", x => x.Phone },
            { "Email", x => x.Email }
        };

    protected override IQueryable<Contractor> ApplyDataSeparation(IQueryable<Contractor> query)
    {
        var statusValues = HttpContextAccessor.HttpContext?.Request?.Query["contractorStatuses"];

        if (statusValues is { Count: > 0 })
        {
            var statuses = ((IEnumerable<string>)statusValues).Select(v => Enum.Parse<ContractorStatus>(v, true)).ToList();
            query = query.Where(c => statuses.Contains(c.ContractorStatus));
        }

        return base.ApplyDataSeparation(query);
    }
}
