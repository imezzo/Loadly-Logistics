using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDriverRepository : IRepository<Common.Entities.Driver>
{
    Task<Common.Entities.Driver?> GetByUserNameAsync(string userName);
    Task<Common.Entities.Driver?> GetByEmailAsync(string email);
    Task<Common.Entities.Driver?> GetByPhoneAsync(string phone);
    Task<Common.Entities.Driver?> GetByLoginAsync(string login);
    Task<Common.Entities.Driver?> GetByNationalIdAsync(string nationalId);
    Task<IReadOnlySet<string>> GetExistingUserNamesAsync(IReadOnlyCollection<string> userNames);
    Task<IReadOnlySet<string>> GetExistingNationalIdsAsync(IReadOnlyCollection<string> nationalIds);
    Task<IReadOnlySet<string>> GetExistingEmailsAsync(IReadOnlyCollection<string> emails);
    Task<IReadOnlySet<string>> GetExistingPhonesAsync(IReadOnlyCollection<string> phones);
    Task<bool> ExistsByUserNameAsync(string userName, long? excludeId = null);
    Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);
    Task<bool> ExistsByPhoneAsync(string phone, long? excludeId = null);
    Task<bool> ExistsByNationalIdAsync(string nationalId, long? excludeId = null);
    Task<Common.Entities.Driver> GetByIdOrThrowAsync(long id);
    Task<Common.Entities.Driver> GetForFacilityVisitOrThrowAsync(long id);
    Task<(List<Common.Entities.Driver> Drivers, int TotalCount)> ListAsync(
        string? search,
        int? status,
        List<long>? contractorIds,
        bool? hasContractor,
        bool? isAvailable,
        LicenseCategory? licenseCategory,
        List<DriverWorkingStatus>? workingStatuses,
        int? page,
        int? pageSize,
        bool? orderByCompletedTripsDescending = null);
    Task UpdateWorkingStatusAsync(long driverId, DriverWorkingStatus status);
    Task<IReadOnlyDictionary<DriverWorkingStatus, int>> GetWorkingStatusCountsAsync();
    Task<IReadOnlyList<(int Month, int DriversCount)>> GetMonthlyAvailableDriversCountAsync(int year);
    Task<int> GetDriversCreatedCountBeforeAsync(DateTime date);
    Task<Dictionary<string, Common.Entities.Driver>> GetMapByNationalIdsAsync(IReadOnlyCollection<string> nationalIds);

    Task<(int ActiveDriversCount, int FreeDriversCount)> GetAvailableActiveDriversCountAsync();
}
