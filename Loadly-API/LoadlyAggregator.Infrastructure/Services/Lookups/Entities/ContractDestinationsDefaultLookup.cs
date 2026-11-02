using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

/// <summary>
/// Contract routes (<see cref="ContractDestination"/>) for one contract.
/// Requires query <c>contractId</c> (long). Admin: any contract. Company: only when the contract belongs to the caller (JWT subject = company id).
/// </summary>
public class ContractDestinationsDefaultLookup : BaseLookup<ContractDestination>
{
    private readonly IContractRepository _contractRepository;

    public ContractDestinationsDefaultLookup(
        IContractDestinationRepository repository,
        IContractRepository contractRepository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
        _contractRepository = contractRepository;
    }

    protected override Expression<Func<ContractDestination, string>> GetValueSelector()
    {
        string language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? cd => cd.NameAr ?? cd.NameEn ?? string.Empty
            : cd => cd.NameEn ?? cd.NameAr ?? string.Empty;
    }

    protected override IQueryable<ContractDestination> ApplyDataSeparation(IQueryable<ContractDestination> query)
    {
        HttpContext? httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
            return query.Where(_ => false);

        string? contractIdRaw = httpContext.Request.Query["contractId"].ToString();
        if (!long.TryParse(contractIdRaw, out long contractId) || contractId <= 0)
            return query.Where(_ => false);

        query = query.Where(cd => cd.ContractId == contractId && cd.Status != ContractDestinationStatus.ARCHIVED);

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
