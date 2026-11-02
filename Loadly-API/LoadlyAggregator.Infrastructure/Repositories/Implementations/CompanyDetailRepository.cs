using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CompanyDetailRepository : Repository<CompanyDetail>, ICompanyDetailRepository
{
    public CompanyDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<CompanyDetail?> GetByCompanyIdAsync(long companyId) =>
        _dbSet.FirstOrDefaultAsync(cd => cd.CompanyId == companyId);

    public Task<CompanyDetail?> GetByNameEnAsync(string nameEn) =>
        _dbSet.FirstOrDefaultAsync(cd => cd.NameEn == nameEn);

    public Task<CompanyDetail?> GetByNameArAsync(string nameAr) =>
        _dbSet.FirstOrDefaultAsync(cd => cd.NameAr == nameAr);

    public Task<CompanyDetail?> GetByRegistrationNumberAsync(string registrationNumber) =>
        _dbSet.FirstOrDefaultAsync(cd => cd.RegistrationNumber == registrationNumber);

    public async Task<IReadOnlySet<string>> GetExistingRegistrationNumbersAsync(IReadOnlyCollection<string> registrationNumbers) =>
    new HashSet<string>(await _dbSet.Where(cd => cd.RegistrationNumber != null && registrationNumbers.Contains(cd.RegistrationNumber)).Select(cd => cd.RegistrationNumber!).ToListAsync());

    public Task<CompanyDetail?> GetByWebsiteUrlAsync(string websiteUrl) =>
        _dbSet.FirstOrDefaultAsync(cd => cd.WebsiteUrl == websiteUrl);

    public async Task<bool> ExistsByNameEnAsync(string nameEn, long? excludeCompanyId = null) =>
        await _dbSet
            .Where(cd => cd.NameEn == nameEn)
            .Where(cd => !excludeCompanyId.HasValue || cd.CompanyId != excludeCompanyId.Value)
            .AnyAsync();

    public async Task<bool> ExistsByNameArAsync(string nameAr, long? excludeCompanyId = null) =>
        await _dbSet
            .Where(cd => cd.NameAr == nameAr)
            .Where(cd => !excludeCompanyId.HasValue || cd.CompanyId != excludeCompanyId.Value)
            .AnyAsync();

    public async Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber, long? excludeCompanyId = null) =>
        await _dbSet
            .Where(cd => cd.RegistrationNumber == registrationNumber)
            .Where(cd => !excludeCompanyId.HasValue || cd.CompanyId != excludeCompanyId.Value)
            .AnyAsync();

    public async Task<bool> ExistsByWebsiteUrlAsync(string websiteUrl, long? excludeCompanyId = null) =>
        await _dbSet
            .Where(cd => cd.WebsiteUrl == websiteUrl)
            .Where(cd => !excludeCompanyId.HasValue || cd.CompanyId != excludeCompanyId.Value)
            .AnyAsync();

    public async Task<Dictionary<string, CompanyDetail>> GetMapByTaxIdsAsync(IReadOnlyCollection<string> taxIds) =>
        (await _dbSet
            .Include(cd => cd.Company)
            .Where(cd => cd.TaxId != null && taxIds.Contains(cd.TaxId))
            .ToListAsync())
        .GroupBy(cd => cd.TaxId!)
        .ToDictionary(g => g.Key, g => g.OrderByDescending(cd => cd.UpdatedAt).First());
}
