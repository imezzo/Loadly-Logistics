using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<(RefreshToken Entity, string PlainToken)> CreateRefreshTokenAsync(
        long modelId,
        string modelType,
        string? deviceInfo = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionName = null);

    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken);

    Task<(RefreshToken Entity, string PlainToken)> RotateRefreshTokenAsync(
        RefreshToken oldToken,
        string? deviceInfo = null,
        string? ipAddress = null,
        string? userAgent = null);

    Task RevokeRefreshTokenAsync(string refreshToken);

    Task RevokeModelTokensAsync(long modelId, string modelType);
    Task RevokeModelTokensBulkAsync(List<long> modelIds, string modelType);

    Task HandleStatusChangeTokenRevocationAsync(List<long> adminIds, int status, int updatedCount, IJwtTokenService jwtTokenService);



    Task RevokeTokenFamilyAsync(string tokenFamilyId);

    Task UpdateLastUsedAsync(long sessionId);
}
public class RefreshTokenValidationResult
{
    public bool IsValid { get; private set; }
    public RefreshToken? Token { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsTheftDetected { get; private set; }
    public static RefreshTokenValidationResult Valid(RefreshToken token)
    {
        return new RefreshTokenValidationResult
        {
            IsValid = true,
            Token = token
        };
    }
    public static RefreshTokenValidationResult Invalid(string message)
    {
        return new RefreshTokenValidationResult
        {
            IsValid = false,
            ErrorMessage = message
        };
    }
    public static RefreshTokenValidationResult TheftDetected(string message)
    {
        return new RefreshTokenValidationResult
        {
            IsValid = false,
            IsTheftDetected = true,
            ErrorMessage = message
        };
    }
}
