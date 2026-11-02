using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

/// <summary>
/// Lookup returning contract Id and ReferenceNumber (Value).
/// Company callers: active contracts for JWT <c>companyId</c> only (same tenant rule as roles lookup).
/// Other callers: all active contracts; optional query param <c>companyId</c> filters by company.
/// </summary>
public class ContractReferenceNumberDefaultLookup : BaseLookup<Contract>
{
    private const string CompanyUserType = "company";
    public ContractReferenceNumberDefaultLookup(
        IContractRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Contract, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? c => (c.ReferenceNumber ?? string.Empty) + " - " + (c.NameAr ?? c.NameEn ?? string.Empty)
            : c => (c.ReferenceNumber ?? string.Empty) + " - " + (c.NameEn ?? c.NameAr ?? string.Empty);
    }

    protected override IQueryable<Contract> ApplyDataSeparation(IQueryable<Contract> query)
    {
        query = query.Where(c => c.Status == ContractStatus.ACTIVE);

        var userType = HttpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value;
        if (string.Equals(userType, CompanyUserType, StringComparison.OrdinalIgnoreCase))
        {
            if (!long.TryParse(HttpContextAccessor.HttpContext?.User?.FindFirst("companyId")?.Value, out var companyId)
                || companyId == 0)
            {
                return query.Where(_ => false);
            }

            return query.Where(c => c.CompanyId == companyId);
        }

        return base.ApplyDataSeparation(query);
    }
}
