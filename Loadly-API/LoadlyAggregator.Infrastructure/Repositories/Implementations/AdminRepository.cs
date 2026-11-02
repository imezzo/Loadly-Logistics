using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class AdminRepository : Repository<Admin>, IAdminRepository
{
    public AdminRepository(ApplicationDbContext context) : base(context)
    {
    }

    private const string AdminModelType = "Admin";

    public override async Task<Admin?> GetByIdAsync(long id) =>
        await _dbSet
            .Where(a => a.Id == id)
            .Include(a => a.UserRoles.Where(mhr => mhr.ModelType == AdminModelType)).ThenInclude(ur => ur.Role)
            .Include(a => a.Addresses.Where(addr => addr.ModelType == AdminModelType).OrderBy(a => a.CreatedAt))
            .Include(a => a.Contacts.Where(c => c.ModelType == AdminModelType).OrderBy(c => c.CreatedAt))
            .FirstOrDefaultAsync();

    public async Task<Admin?> GetByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.FirstOrDefaultAsync(a => a.UserName == normalizedUserName);
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.FirstOrDefaultAsync(a => a.Email == normalizedEmail);
    }

    public async Task<Admin?> GetByPhoneAsync(string phone) =>
        await _dbSet.FirstOrDefaultAsync(a => a.Phone == phone);

    public async Task<Admin?> GetByLoginAsync(string login)
    {
        var normalizedUserName = login.ToLower();
        return await _dbSet
            .FirstOrDefaultAsync(a => a.UserName == normalizedUserName || a.Phone == login);
    }

    public async Task<Admin> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new Shared.Exceptions.NotFoundException("admins.not_found_with_id", id);

    public async Task<bool> ExistsByUserNameAsync(string userName, long excludeId)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.AnyAsync(a => a.UserName == normalizedUserName && a.Id != excludeId);
    }

    public async Task<bool> ExistsByEmailAsync(string email, long excludeId)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.AnyAsync(a => a.Email == normalizedEmail && a.Id != excludeId);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, long excludeId) =>
        await _dbSet.AnyAsync(a => a.Phone == phone && a.Id != excludeId);

    public override async Task AddAsync(Admin admin) =>
        await base.AddAsync(admin);

    public override void Remove(Admin admin) =>
        base.Remove(admin);

    public override IQueryable<Admin> Query() =>
        base.Query();

    public async Task<int> BulkUpdateStatusAsync(List<long> adminIds, UserStatus status)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(a => (adminIds ?? new List<long>()).Contains(a.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Status, status)
                .SetProperty(a => a.UpdatedAt, now));
    }

    public async Task<List<long>> GetExistingIdsAsync(List<long> adminIds) =>
        await _dbSet
            .Where(a => (adminIds ?? new List<long>()).Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();

    public async Task<(List<Admin> Admins, int TotalCount)> ListAsync(
        string? search,
        int? type,
        int? status,
        long? excludeUserId,
        int page,
        int pageSize,
        List<long>? roleIds = null)
    {
        var filtered = _dbSet.AsQueryable()
            .Where(a => a.Type != AdminType.SUPER)
            .Where(a => !excludeUserId.HasValue || a.Id != excludeUserId.Value)
            .Where(a => string.IsNullOrWhiteSpace(search) ||
                a.FullName.Contains(search!) ||
                a.UserName.Contains(search!.Trim().ToLower()) ||
                a.Email.Contains(search!.Trim().ToLower()) ||
                a.Phone.Contains(search!))
            .Where(a => !type.HasValue || (int)a.Type == type.Value)
            .Where(a => !status.HasValue || (int)a.Status == status.Value)
            .Where(a => roleIds == null || roleIds.Count == 0 ||
                a.UserRoles.Any(mhr =>
                    mhr.ModelType == AdminModelType &&
                    roleIds.Contains(mhr.RoleId)));
        var totalCount = await filtered.CountAsync();
        var admins = await filtered
            .Include(a => a.UserRoles.Where(mhr => mhr.ModelType == AdminModelType)).ThenInclude(ur => ur.Role)
            .Include(a => a.Addresses.Where(addr => addr.ModelType == AdminModelType).OrderBy(a => a.CreatedAt))
            .Include(a => a.Contacts.Where(c => c.ModelType == AdminModelType).OrderBy(c => c.CreatedAt))
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (admins, totalCount);
    }
}
