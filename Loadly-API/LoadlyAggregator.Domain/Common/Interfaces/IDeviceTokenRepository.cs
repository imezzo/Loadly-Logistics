using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IDeviceTokenRepository : IRepository<DeviceToken>
{
    Task<DeviceToken?> GetByFcmTokenAsync(string fcmToken);
    Task<List<DeviceToken>> GetByUserAsync(long userId, string userType);
    Task<List<DeviceToken>> GetActiveTokensByUserAsync(long userId, string userType);
    Task<DeviceToken?> GetByUserAndTokenAsync(long userId, string userType, string fcmToken);
    Task<DeviceToken?> GetByUserAndDeviceIdAsync(long userId, string userType, string deviceId);
    Task DeactivateTokenAsync(long id);
    Task DeactivateUserTokensAsync(long userId, string userType);
    Task DeactivateTokenByFcmTokenAsync(string fcmToken);
    Task UpdateLastUsedAsync(long id);
}
