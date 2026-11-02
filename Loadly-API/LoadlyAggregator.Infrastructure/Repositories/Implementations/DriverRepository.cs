using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DriverRepository : Repository<Driver>, IDriverRepository
{
    private const string DriverModelType = "Driver";

    public DriverRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Driver?> GetByIdAsync(long id) =>
        await DriverVehicleIncludes(_dbSet)
            .Include(d => d.Contractor)
            .Include(d => d.Addresses.Where(a => a.ModelType == DriverModelType).OrderBy(a => a.CreatedAt))
            .Include(d => d.Contacts.Where(c => c.ModelType == DriverModelType).OrderBy(c => c.CreatedAt))
            .Include(d => d.Trips)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Driver?> GetByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet.FirstOrDefaultAsync(d => d.UserName == normalizedUserName);
    }

    public async Task<Driver?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet.FirstOrDefaultAsync(d => d.Email == normalizedEmail);
    }

    public async Task<Driver?> GetByPhoneAsync(string phone) =>
        await _dbSet.FirstOrDefaultAsync(d => d.Phone == phone);

    public async Task<Driver?> GetByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;
        var normalizedUserNameOrEmail = login.Trim().ToLower();
        return await _dbSet.FirstOrDefaultAsync(d =>
            d.UserName == normalizedUserNameOrEmail || d.Email == normalizedUserNameOrEmail || d.Phone == login.Trim());
    }

    public async Task<Driver?> GetByNationalIdAsync(string nationalId) =>
        await _dbSet.FirstOrDefaultAsync(d => d.NationalId == nationalId);

    public async Task<IReadOnlySet<string>> GetExistingUserNamesAsync(IReadOnlyCollection<string> userNames)
    {
        var normalizedUserNames = userNames.Select(u => u.ToLower()).ToList();
        return new HashSet<string>(await _dbSet.Where(d => normalizedUserNames.Contains(d.UserName)).Select(d => d.UserName).ToListAsync());
    }

    public async Task<IReadOnlySet<string>> GetExistingNationalIdsAsync(IReadOnlyCollection<string> nationalIds) =>
        new HashSet<string>(await _dbSet.Where(d => d.NationalId != null && nationalIds.Contains(d.NationalId)).Select(d => d.NationalId!).ToListAsync());

    public async Task<IReadOnlySet<string>> GetExistingEmailsAsync(IReadOnlyCollection<string> emails)
    {
        var normalizedEmails = emails.Select(e => e.ToLower()).ToList();
        return new HashSet<string>(await _dbSet.Where(d => normalizedEmails.Contains(d.Email)).Select(d => d.Email).ToListAsync());
    }

    public async Task<IReadOnlySet<string>> GetExistingPhonesAsync(IReadOnlyCollection<string> phones) =>
        new HashSet<string>(await _dbSet.Where(d => phones.Contains(d.Phone)).Select(d => d.Phone).ToListAsync());

    public async Task<bool> ExistsByUserNameAsync(string userName, long? excludeId = null)
    {
        var normalizedUserName = userName.ToLower();
        return await _dbSet
            .Where(d => d.UserName == normalizedUserName)
            .Where(d => !excludeId.HasValue || d.Id != excludeId.Value)
            .AnyAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, long? excludeId = null)
    {
        var normalizedEmail = email.ToLower();
        return await _dbSet
            .Where(d => d.Email == normalizedEmail)
            .Where(d => !excludeId.HasValue || d.Id != excludeId.Value)
            .AnyAsync();
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, long? excludeId = null) =>
        await _dbSet
            .Where(d => d.Phone == phone)
            .Where(d => !excludeId.HasValue || d.Id != excludeId.Value)
            .AnyAsync();

    public async Task<bool> ExistsByNationalIdAsync(string nationalId, long? excludeId = null) =>
        await _dbSet
            .Where(d => d.NationalId == nationalId)
            .Where(d => !excludeId.HasValue || d.Id != excludeId.Value)
            .AnyAsync();

    public async Task<Driver> GetByIdOrThrowAsync(long id) =>
        await GetByIdAsync(id) ?? throw new NotFoundException("drivers.not_found_with_id", id);

    public async Task<Driver> GetForFacilityVisitOrThrowAsync(long id) =>
        await _dbSet.FirstOrDefaultAsync(d => d.Id == id)
        ?? throw new NotFoundException("drivers.not_found_with_id", id);

    public async Task<(List<Driver> Drivers, int TotalCount)> ListAsync(
        string? search,
        int? status,
        List<long>? contractorIds,
        bool? hasContractor,
        bool? isAvailable,
        LicenseCategory? licenseCategory,
        List<DriverWorkingStatus>? workingStatuses,
        int? page,
        int? pageSize,
        bool? orderByCompletedTripsDescending = null)
    {
        var query = _dbSet.AsQueryable()
            .Where(d => string.IsNullOrWhiteSpace(search) ||
                (d.FullName != null && d.FullName.Contains(search!)) ||
                (d.FullNameAr != null && d.FullNameAr.Contains(search!)) ||
                (d.NationalId != null && d.NationalId.Contains(search!)) ||
                d.UserName.Contains(search!) ||
                d.Email.Contains(search!) ||
                d.Phone.Contains(search!)
            )
            .Where(d => !status.HasValue || (int)d.Status == status.Value)
            .Where(d => contractorIds == null || contractorIds.Count == 0 ||
                (d.ContractorId.HasValue && contractorIds.Contains(d.ContractorId.Value)))
            .Where(d => !hasContractor.HasValue ||
                (hasContractor.Value ? d.ContractorId != null : d.ContractorId == null))
            .Where(d => !isAvailable.HasValue ||
                (isAvailable.Value
                    ? !_context.Vehicles.Any(v => v.DriverId == d.Id)
                    : _context.Vehicles.Any(v => v.DriverId == d.Id)))
            .Where(d => !licenseCategory.HasValue ||
                _context.DriverDetails.Any(dd => dd.DriverId == d.Id && dd.LicenseCategory == licenseCategory.Value))
            .Where(d => workingStatuses == null || workingStatuses.Count == 0 ||
                (d.WorkingStatus != null && workingStatuses.Contains(d.WorkingStatus.Value)));

        IQueryable<Driver> ordered = orderByCompletedTripsDescending == true
            ? query.OrderByDescending(d => d.Trips.Count(t =>
                t.Status == TripStatus.Completed))
            : query.OrderByDescending(d => d.CreatedAt);

        IQueryable<Driver> withIncludes = DriverVehicleIncludes(ordered
            .AsNoTracking()
            .AsSplitQuery())
            .Include(d => d.Contractor)
            .Include(d => d.Addresses.Where(a => a.ModelType == DriverModelType).OrderBy(a => a.CreatedAt))
            .Include(d => d.Contacts.Where(c => c.ModelType == DriverModelType).OrderBy(c => c.CreatedAt))
            .Include(d => d.Trips)
            .Include(d => d.DriverDetail)
            .Include(d => d.DriverDocuments.OrderByDescending(dd => dd.CreatedAt))
                .ThenInclude(dd => dd.Document);

        if (page is null || pageSize is null)
        {
            var allDrivers = await withIncludes.ToListAsync();
            return (allDrivers, 0);
        }

        int totalCount = await ordered.CountAsync();
        var drivers = await withIncludes
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        return (drivers, totalCount);
    }

    public async Task UpdateWorkingStatusAsync(long driverId, DriverWorkingStatus status)
    {
        var now = DateTime.UtcNow;
        await _dbSet
            .Where(d => d.Id == driverId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.WorkingStatus, status)
                .SetProperty(d => d.UpdatedAt, now));
    }

    public async Task<IReadOnlyDictionary<DriverWorkingStatus, int>> GetWorkingStatusCountsAsync()
    {
        var rows = await _dbSet
            .GroupBy(d => d.WorkingStatus ?? DriverWorkingStatus.Available)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return rows.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task<IReadOnlyList<(int Month, int DriversCount)>> GetMonthlyAvailableDriversCountAsync(int year) =>
        (await _dbSet
            .Where(d => d.CreatedAt >= new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                     && d.CreatedAt < new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .GroupBy(d => d.CreatedAt.Month)
            .Select(g => new { Month = g.Key, DriversCount = g.Count() })
            .ToListAsync())
            .Select(x => (x.Month, x.DriversCount))
            .ToList();

    public async Task<int> GetDriversCreatedCountBeforeAsync(DateTime date) =>
        await _dbSet.CountAsync(d => d.CreatedAt < date);

    public async Task<Dictionary<string, Driver>> GetMapByNationalIdsAsync(IReadOnlyCollection<string> nationalIds) =>
        (await _dbSet
            .Where(d => !string.IsNullOrEmpty(d.NationalId) && nationalIds.Contains(d.NationalId))
            .ToListAsync())
        .GroupBy(d => d.NationalId)
        .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.UpdatedAt).First());

    public async Task<(int ActiveDriversCount, int FreeDriversCount)> GetAvailableActiveDriversCountAsync()
    {
        var result = await _dbSet
            .Where(d => d.Status == UserStatus.ACTIVE)
            .GroupBy(d => d.Status)
            .Select(g => new
            {
                ActiveDriversCount = g.Count(),
                FreeDriversCount = g.Count(d => d.WorkingStatus == DriverWorkingStatus.Available)
            })
            .FirstOrDefaultAsync();

        return (result?.ActiveDriversCount ?? 0, result?.FreeDriversCount ?? 0);
    }

    private static IQueryable<Driver> DriverVehicleIncludes(IQueryable<Driver> query) =>
        query
            .Include(d => d.Vehicles)
                .ThenInclude(v => v.VehicleType)
            .Include(d => d.Vehicles)
                .ThenInclude(v => v.VehicleBrandModel!)
                    .ThenInclude(bm => bm.VehicleBrand)
            .Include(d => d.Vehicles)
                .ThenInclude(v => v.Sector)
            .Include(d => d.Vehicles)
                .ThenInclude(v => v.TailVehicles);
}
