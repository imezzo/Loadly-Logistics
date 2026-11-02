using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class DriversDefaultLookup(
    IDriverRepository repository,
    IVehicleRepository vehicleRepository,
    ICompanyRepository companyRepository,
    ILanguageService languageService,
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserService currentUserService) : BaseLookup<Driver>(repository, languageService, httpContextAccessor)
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    protected override Expression<Func<Driver, string>> GetValueSelector()
    {
        string language = LanguageService.GetCurrentLanguage();
        return language == "ar"
            ? x => x.FullNameAr ?? x.FullName ?? string.Empty
            : x => x.FullName ?? x.FullNameAr ?? string.Empty;
    }

    protected override Dictionary<string, Expression<Func<Driver, object?>>> GetAdditionalFields()
        => new()
        {
            { "Phone", x => x.Phone },
            { "Email", x => x.Email }
        };

    protected override IQueryable<Driver> ApplyDataSeparation(IQueryable<Driver> query)
    {
        string? contractIdRaw = HttpContextAccessor.HttpContext?.Request.Query["contractId"].ToString();

        bool hasContractId = long.TryParse(contractIdRaw, out long contractId) && contractId > 0;

        var userType = HttpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value;

        if (string.Equals(userType, "company", StringComparison.OrdinalIgnoreCase))
        {

            var companyUserIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (companyUserIdClaim == null || !long.TryParse(companyUserIdClaim.Value, out long companyUserId))
            {
                return query.Where(_ => false);
            }

            var companyIds = _companyRepository.Query()
                .Where(c => c.Id == companyUserId)
                .Select(c => c.CompanyId ?? c.Id);

            var vehicleQuery = _vehicleRepository.Query()
                .Where(v =>
                    v.DriverId != null &&
                    v.ContractId != null &&
                    v.Contract != null &&
                    companyIds.Contains(v.Contract.CompanyId) &&
                    v.Contract.Status == ContractStatus.ACTIVE);

            if (hasContractId)
                vehicleQuery = vehicleQuery.Where(v => v.ContractId == contractId);

            var driverIdsOnCompanyContractVehicles = vehicleQuery
                .Select(v => v.DriverId!.Value)
                .Distinct();

            query = query.Where(d => driverIdsOnCompanyContractVehicles.Contains(d.Id));

            return base.ApplyDataSeparation(query);
        }

        if (!hasContractId)
            return base.ApplyDataSeparation(query);

        IQueryable<long> driverIdsOnContractVehicles = _vehicleRepository.Query()
            .Where(v => v.DriverId != null && v.ContractId == contractId)
            .Select(v => v.DriverId!.Value)
            .Distinct();

        query = query.Where(d => driverIdsOnContractVehicles.Contains(d.Id));

        return base.ApplyDataSeparation(query);
    }
}
