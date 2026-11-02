using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private const string CompanyModelType = "Company";

    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Company?> GetByIdAsync(long id) =>
        await _dbSet
            .Include(c => c.CompanyDetail)
            .Include(c => c.CompanyDocuments).ThenInclude(cd => cd.Document)
            .Include(c => c.Addresses.Where(a => a.ModelType == CompanyModelType).OrderBy(a => a.CreatedAt))
            .Include(c => c.Contacts.Where(a => a.ModelType == CompanyModelType).OrderBy(c => c.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Company?> GetByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.UserName == normalizedUserName);
    }

    public async Task<Company?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == normalizedEmail);
    }

    public async Task<Company?> GetByPhoneAsync(string phone) =>
        await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);

    public async Task<Company?> GetByLoginAsync(string login)
    {
        var normalizedUserNameOrEmail = login.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c =>
            c.UserName == normalizedUserNameOrEmail || c.Email == normalizedUserNameOrEmail || c.Phone == login);
    }

    public async Task<IReadOnlySet<string>> GetExistingUserNamesAsync(IReadOnlyCollection<string> userNames)
    {
        var normalizedUserNames = userNames.Select(u => u.ToLower()).ToList();
        return new HashSet<string>(await _dbSet.Where(c => normalizedUserNames.Contains(c.UserName)).Select(c => c.UserName).ToListAsync());
    }

    public async Task<IReadOnlySet<string>> GetExistingEmailsAsync(IReadOnlyCollection<string> emails)
    {
        var normalizedEmails = emails.Select(e => e.ToLower()).ToList();
        return new HashSet<string>(await _dbSet.Where(c => normalizedEmails.Contains(c.Email)).Select(c => c.Email).ToListAsync());
    }

    public async Task<IReadOnlySet<string>> GetExistingPhonesAsync(IReadOnlyCollection<string> phones) =>
        new HashSet<string>(await _dbSet.Where(c => phones.Contains(c.Phone)).Select(c => c.Phone).ToListAsync());

    public async Task<Company> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("companies.not_found_with_id", id);

    public async Task<Company> GetUserByIdOrThrowAsync(long id) =>
        await _dbSet
            .Include(u => u.ParentCompany)
                .ThenInclude(c => c!.CompanyDetail)
            .Include(u => u.UserRoles.Where(mhr => mhr.ModelType == CompanyModelType))
                .ThenInclude(mhr => mhr.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rhp => rhp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id)
        ?? throw new NotFoundException("users.not_found_with_id", id);

    private static IQueryable<Company> ApplyListFilters(IQueryable<Company> query, string? search, int? status) =>
        query
            .Where(c => c.CompanyId == null)
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.FullName.Contains(search!) ||
                c.UserName.Contains(search!.Trim().ToLower()) ||
                c.Email.Contains(search!.Trim().ToLower()) ||
                c.Phone.Contains(search!))
            .Where(c => !status.HasValue || (int)c.Status == status.Value);

    public async Task<(List<Company> Companies, int TotalCount)> ListAsync(
        string? search,
        int? status,
        int page,
        int pageSize)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, status);
        var totalCount = await filtered.CountAsync();
        var companies = await filtered
            .Include(c => c.CompanyDetail)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (companies, totalCount);
    }

    public async Task<(List<Company> Companies, int TotalCount)> ListWithChildrenAsync(
        string? search,
        int? status,
        int page,
        int pageSize)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, status);
        var totalCount = await filtered.CountAsync();
        var companies = await filtered
            .Include(c => c.CompanyDetail)
            .Include(c => c.CompanyDocuments).ThenInclude(cd => cd.Document)
            .Include(c => c.Addresses.Where(a => a.ModelType == CompanyModelType).OrderBy(a => a.CreatedAt))
            .Include(c => c.Contacts.Where(a => a.ModelType == CompanyModelType).OrderBy(c => c.CreatedAt))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (companies, totalCount);
    }

    public async Task<(List<Company> Users, int TotalCount)> ListUsersOfCompanyAsync(
    long companyId,
    string? search,
    int? status,
    int page,
    int pageSize)
    {
        var normalizedSearch = search?.Trim().ToLower();
        var query = _dbSet.AsQueryable()
            .Where(c => c.CompanyId == companyId && c.Type == CompanyType.EMPLOYEE)
            .Where(c => string.IsNullOrWhiteSpace(normalizedSearch) ||
                c.FullName.Contains(normalizedSearch) ||
                c.UserName.Contains(normalizedSearch) ||
                c.Email.Contains(normalizedSearch) ||
                c.Phone.Contains(normalizedSearch))
            .Where(c => !status.HasValue || (int)c.Status == status.Value);

        var users = await query
            .Include(c => c.ParentCompany)
            .ThenInclude(c => c!.CompanyDetail)
            .Include(c => c.UserRoles.Where(mhr => mhr.ModelType == CompanyModelType))
                .ThenInclude(mhr => mhr.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rhp => rhp.Permission)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await query.CountAsync();
        return (users, totalCount);
    }
}
