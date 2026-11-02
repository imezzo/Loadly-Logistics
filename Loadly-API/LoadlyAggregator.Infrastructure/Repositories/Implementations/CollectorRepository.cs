using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class CollectorRepository : Repository<Collector>, ICollectorRepository
{
    private const string CollectorModelType = "Collector";

    public CollectorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Collector?> GetByIdAsync(long id) =>
        await _dbSet
            .Where(c => c.Id == id)
            .Include(c => c.UserRoles.Where(mhr => mhr.ModelType == CollectorModelType)).ThenInclude(ur => ur.Role)
            .Include(c => c.Addresses.Where(addr => addr.ModelType == CollectorModelType).OrderBy(a => a.CreatedAt))
            .Include(c => c.Contacts.Where(contact => contact.ModelType == CollectorModelType).OrderBy(c => c.CreatedAt))
            .FirstOrDefaultAsync();

    public async Task<Collector?> GetByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.UserName == normalizedUserName);
    }

    public async Task<Collector?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == normalizedEmail);
    }

    public async Task<Collector?> GetByPhoneAsync(string phone) =>
        await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);

    public async Task<Collector?> GetByLoginAsync(string login) =>
        await _dbSet.FirstOrDefaultAsync(c =>
            c.UserName == login.Trim().ToLower() || c.Phone == login.Trim());

    public async Task<Collector> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new Shared.Exceptions.NotFoundException("collectors.not_found_with_id", id);

    public async Task<bool> ExistsByUserNameAsync(string userName, long excludeId)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.AnyAsync(c => c.UserName == normalizedUserName && c.Id != excludeId);
    }

    public async Task<bool> ExistsByEmailAsync(string email, long excludeId)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.AnyAsync(c => c.Email == normalizedEmail && c.Id != excludeId);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, long excludeId) =>
        await _dbSet.AnyAsync(c => c.Phone == phone && c.Id != excludeId);

    public async Task<int> BulkUpdateStatusAsync(List<long> collectorIds, UserStatus status)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(c => (collectorIds ?? new List<long>()).Contains(c.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.Status, status)
                .SetProperty(c => c.UpdatedAt, now));
    }

    public async Task<List<long>> GetExistingIdsAsync(List<long> collectorIds) =>
        await _dbSet
            .Where(c => (collectorIds ?? new List<long>()).Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync();

    public async Task<(List<Collector> Collectors, int TotalCount)> ListAsync(
        string? search,
        int? status,
        long? excludeUserId,
        int page,
        int pageSize)
    {
        var filtered = _dbSet.AsQueryable()
            .Where(c => !excludeUserId.HasValue || c.Id != excludeUserId.Value)
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.FullName.Contains(search!) ||
                c.UserName.Contains(search!.Trim().ToLower()) ||
                c.Email.Contains(search!.Trim().ToLower()) ||
                c.Phone.Contains(search!))
            .Where(c => !status.HasValue || (int)c.Status == status.Value);

        var totalCount = await filtered.CountAsync();
        var collectors = await filtered
            .Include(c => c.UserRoles.Where(mhr => mhr.ModelType == CollectorModelType)).ThenInclude(ur => ur.Role)
            .Include(c => c.Addresses.Where(addr => addr.ModelType == CollectorModelType).OrderBy(a => a.CreatedAt))
            .Include(c => c.Contacts.Where(contact => contact.ModelType == CollectorModelType).OrderBy(c => c.CreatedAt))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (collectors, totalCount);
    }
}
