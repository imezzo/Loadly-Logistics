using System.Security.Claims;

namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IJwtTokenService
{

    string GenerateToken(long userId, string userType, string email, List<string>? roles = null, List<string>? permissions = null, string? tokenFamilyId = null, long? companyId = null);
    string GenerateTemporaryMfaToken(long userId, string userType, string email, int expiresInMinutes = 5);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    string GenerateTokenFamilyId();


    ClaimsPrincipal? ValidateToken(string token);





    Task RevokeTokenAsync(string jti, int expirationMinutes = 120);




    Task<bool> IsRevokedAsync(string jti);




    Task RevokeAllUserTokensAsync(long userId, string userType, int expirationMinutes = 120);




    Task<bool> AreAllUserTokensRevokedAsync(long userId, string userType);





    Task ClearUserRevocationAsync(long userId, string userType);
    Task ClearUserRevocationBulkAsync(IEnumerable<long> userIds, string userType);





    Task RevokeTokenFamilyAsync(string tokenFamilyId, int expirationMinutes = 120);




    Task<bool> IsTokenFamilyRevokedAsync(string tokenFamilyId);
}

