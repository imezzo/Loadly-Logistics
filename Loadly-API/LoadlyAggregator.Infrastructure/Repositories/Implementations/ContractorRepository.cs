using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class ContractorRepository : Repository<Contractor>, IContractorRepository
{
    public ContractorRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Contractor?> GetByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        var normalizedUserName = userName.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.UserName == normalizedUserName);
    }

    public async Task<Contractor?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalizedEmail = email.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == normalizedEmail);
    }

    public async Task<Contractor?> GetByPhoneAsync(string phone) =>
        await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);

    public async Task<Contractor?> GetBySecondPhoneNumberAsync(string secondPhoneNumber) =>
        await _dbSet.FirstOrDefaultAsync(c => c.SecondPhoneNumber == secondPhoneNumber);

    public async Task<Contractor?> GetByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        var normalizedLogin = login.ToLower();
        return await _dbSet.FirstOrDefaultAsync(c =>
            c.DeletedAt == null &&
            (c.UserName == normalizedLogin || c.Email == normalizedLogin || c.Phone == login));
    }

    public async Task<Contractor?> GetByNationalIdAsync(string nationalId) =>
        await _dbSet.FirstOrDefaultAsync(c => c.NationalId == nationalId);

    public async Task<bool> ExistsByUserNameAsync(string userName, long excludeId)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        var normalizedUserName = userName.ToLower();
        return await _dbSet.AnyAsync(c =>
            c.UserName == normalizedUserName &&
            c.Id != excludeId);
    }

    public async Task<bool> ExistsByEmailAsync(string email, long excludeId)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var normalizedEmail = email.ToLower();
        return await _dbSet.AnyAsync(c =>
            c.Email == normalizedEmail &&
            c.Id != excludeId);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, long excludeId) =>
        await _dbSet.AnyAsync(c => c.Phone == phone && c.Id != excludeId);

    public async Task<bool> ExistsBySecondPhoneNumberAsync(string secondPhoneNumber, long excludeId) =>
        await _dbSet.AnyAsync(c => c.SecondPhoneNumber == secondPhoneNumber && c.Id != excludeId);

    public async Task<bool> ExistsByNationalIdAsync(string nationalId, long excludeId) =>
        await _dbSet.AnyAsync(c => c.NationalId == nationalId && c.Id != excludeId);

    public async Task<Contractor> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("contractors.not_found_with_id", id);

    public async Task<Contractor> GetByIdWithChildrenOrThrowAsync(long id) =>
        await GetByIdWithChildrenAsync(id)
        ?? throw new NotFoundException("contractors.not_found_with_id", id);

    public async Task<ContractorAggregateCounts> GetAggregateCountsAsync(long contractorId) =>
        await _dbSet
            .AsNoTracking()
            .Where(c => c.Id == contractorId && c.DeletedAt == null)
            .Select(c => new ContractorAggregateCounts(
                c.Drivers.Count,
                c.Vehicles.Count,
                c.Trips.Count,
                c.Trips.Count(t => t.Status == TripStatus.Completed),
                c.Trips.Count(t => t.Status == TripStatus.Completed
                    && t.ContractorPayoutId != null
                    && t.ContractorPayout != null
                    && t.ContractorPayout.Status == ContractorPayoutStatus.Paid),
                c.Trips.Count(t => t.Status == TripStatus.Completed && t.ContractorPayoutId == null),
                c.Trips.Count(t => t.Status == TripStatus.Completed
                    && t.ContractorPayoutId != null
                    && t.ContractorPayout != null
                    && t.ContractorPayout.Status == ContractorPayoutStatus.Pending)))
            .FirstOrDefaultAsync()
        ?? new ContractorAggregateCounts(0, 0, 0, 0, 0, 0, 0);

    public async Task<Dictionary<long, ContractorAggregateCounts>> GetAggregateCountsByIdsAsync(
        IReadOnlyList<long> contractorIds,
        DateTime? tripStartDate = null,
        DateTime? tripEndDate = null)
    {
        if (contractorIds.Count == 0)
        {
            return new Dictionary<long, ContractorAggregateCounts>();
        }

        return await _dbSet
            .AsNoTracking()
            .Where(c => contractorIds.Contains(c.Id) && c.DeletedAt == null)
            .Select(c => new
            {
                c.Id,
                Counts = new ContractorAggregateCounts(
                    c.Drivers.Count,
                    c.Vehicles.Count,
                    c.Trips.Count(t =>
                        (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                        && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))),
                    c.Trips.Count(t => t.Status == TripStatus.Completed
                        && (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                        && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))),
                    c.Trips.Count(t => t.Status == TripStatus.Completed
                        && t.ContractorPayoutId != null
                        && t.ContractorPayout != null
                        && t.ContractorPayout.Status == ContractorPayoutStatus.Paid
                        && (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                        && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))),
                    c.Trips.Count(t => t.Status == TripStatus.Completed
                        && t.ContractorPayoutId == null
                        && (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                        && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))),
                    c.Trips.Count(t => t.Status == TripStatus.Completed
                        && t.ContractorPayoutId != null
                        && t.ContractorPayout != null
                        && t.ContractorPayout.Status == ContractorPayoutStatus.Pending
                        && (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                        && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))))
            })
            .ToDictionaryAsync(x => x.Id, x => x.Counts);
    }

    public async Task<(List<Contractor> Contractors, int TotalCount)> ListWithChildrenAsync(
        string? search,
        ContractorStatus? contractorStatus,
        UserStatus? status,
        int page,
        int pageSize,
        DateTime? tripStartDate = null,
        DateTime? tripEndDate = null)
    {
        var filtered = ApplyListFilters(_dbSet.AsQueryable(), search, contractorStatus, status, tripStartDate, tripEndDate);
        var totalCount = await filtered.CountAsync();
        var contractors = await filtered
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.ContractorDocuments).ThenInclude(cd => cd.Document)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (contractors, totalCount);
    }

    private async Task<Contractor?> GetByIdWithChildrenAsync(long id) =>
        await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.ContractorDocuments).ThenInclude(cd => cd.Document)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

    private static IQueryable<Contractor> ApplyListFilters(
        IQueryable<Contractor> query,
        string? search,
        ContractorStatus? contractorStatus,
        UserStatus? status,
        DateTime? tripStartDate,
        DateTime? tripEndDate) =>
        query
            .Where(c => c.DeletedAt == null)
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                c.FullName.Contains(search!) ||
                c.Phone.Contains(search!) ||
                (c.NationalId != null && c.NationalId.Contains(search!)) ||
                (c.SecondPhoneNumber != null && c.SecondPhoneNumber.Contains(search!)) ||
                (c.Email != null && c.Email != string.Empty && c.Email.Contains(search!)) ||
                (c.UserName != null && c.UserName != string.Empty && c.UserName.Contains(search!)))
            .Where(c => contractorStatus == null || c.ContractorStatus == contractorStatus)
            .Where(c => status == null || c.Status == status)
            .Where(c => (!tripStartDate.HasValue && !tripEndDate.HasValue)
                || c.Trips.Any(t =>
                    (!tripStartDate.HasValue || (t.StartDate.HasValue && t.StartDate.Value >= tripStartDate.Value))
                    && (!tripEndDate.HasValue || (t.EndDate.HasValue && t.EndDate.Value <= tripEndDate.Value))));
}
