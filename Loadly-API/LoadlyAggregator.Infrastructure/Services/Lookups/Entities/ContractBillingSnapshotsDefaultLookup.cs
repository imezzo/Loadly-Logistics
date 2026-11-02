using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

/// <summary>
/// Contract billing snapshots for one contract.
/// Requires query <c>contractId</c> (long). Admin: any contract. Company: only when the contract belongs to the caller.
/// </summary>
public class ContractBillingSnapshotsDefaultLookup : BaseLookup<ContractBillingSnapshot>
{
    private readonly IContractRepository _contractRepository;

    public ContractBillingSnapshotsDefaultLookup(
        IContractBillingSnapshotRepository repository,
        IContractRepository contractRepository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
        _contractRepository = contractRepository;
    }

    protected override Expression<Func<ContractBillingSnapshot, string>> GetValueSelector() =>
        snapshot => snapshot.Name;

    protected override Dictionary<string, Expression<Func<ContractBillingSnapshot, object?>>> GetAdditionalFields() =>
        new()
        {
            ["Code"] = snapshot => snapshot.Status.ToString()
        };

    protected override IQueryable<ContractBillingSnapshot> ApplyDataSeparation(IQueryable<ContractBillingSnapshot> query)
    {
        HttpContext? httpContext = HttpContextAccessor.HttpContext;
        if (httpContext is null)
            return query.Where(_ => false);

        string? contractIdRaw = httpContext.Request.Query["contractId"].ToString();
        if (!long.TryParse(contractIdRaw, out long contractId) || contractId <= 0)
            return query.Where(_ => false);

        query = query.Where(snapshot => snapshot.ContractId == contractId);

        string? userType = httpContext.User?.FindFirst("userType")?.Value;
        if (string.Equals(userType, "admin", StringComparison.OrdinalIgnoreCase))
            return base.ApplyDataSeparation(query);

        if (string.Equals(userType, "company", StringComparison.OrdinalIgnoreCase))
        {
            string? userIdRaw = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdRaw, out long companyId))
                return query.Where(_ => false);

            query = query.Where(_ =>
                _contractRepository.Query().Any(c => c.Id == contractId && c.CompanyId == companyId));
            return base.ApplyDataSeparation(query);
        }

        return query.Where(_ => false);
    }
}
