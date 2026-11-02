using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICompanyDetailRepository : IRepository<Entities.CompanyDetail>
{
    Task<Entities.CompanyDetail?> GetByCompanyIdAsync(long companyId);
    Task<CompanyDetail?> GetByNameEnAsync(string nameEn);
    Task<CompanyDetail?> GetByNameArAsync(string nameAr);
    Task<CompanyDetail?> GetByRegistrationNumberAsync(string registrationNumber);
    Task<IReadOnlySet<string>> GetExistingRegistrationNumbersAsync(IReadOnlyCollection<string> registrationNumbers);
    Task<CompanyDetail?> GetByWebsiteUrlAsync(string websiteUrl);
    Task<bool> ExistsByNameEnAsync(string nameEn, long? excludeCompanyId = null);
    Task<bool> ExistsByNameArAsync(string nameAr, long? excludeCompanyId = null);
    Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber, long? excludeCompanyId = null);
    Task<bool> ExistsByWebsiteUrlAsync(string websiteUrl, long? excludeCompanyId = null);
    Task<Dictionary<string, CompanyDetail>> GetMapByTaxIdsAsync(IReadOnlyCollection<string> taxIds);
}
