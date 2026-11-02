using System.Linq;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Infrastructure.Filters.Base;
using LoadlyAggregator.Infrastructure.Filters.Implementations;

namespace LoadlyAggregator.Infrastructure.Filters.Example;

public class ContractsFilterUtility : BaseFilterUtility<Contract>
{
    public ContractsFilterUtility(IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor)
    {
    }

    protected override IEnumerable<IFilter<Contract>> GetFilters()
    {
        return new IFilter<Contract>[]
        {
            new SearchFilter<Contract>(
                c => c.NameEn,
                c => c.NameAr ?? string.Empty
            ),

            new EqualityFilter<Contract, long>("companyId", c => c.CompanyId),
            new EqualityFilter<Contract, int>("contractType", c => (int)c.ContractType),
            new EqualityFilter<Contract, int>("status", c => (int)c.Status),

            new DateRangeFilter<Contract>(
                c => c.CreatedAt,
                HttpContextAccessor,
                fromKey: "from",
                toKey: "to"
            )
        };
    }
}
