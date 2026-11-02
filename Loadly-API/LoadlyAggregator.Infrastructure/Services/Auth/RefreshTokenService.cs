using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;
    private readonly Domain.Common.Interfaces.IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly int _refreshTokenDays;

    public RefreshTokenService(
        IRefreshTokenRepository repository,
        Domain.Common.Interfaces.IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _timeProvider = timeProvider;
        _refreshTokenDays = int.Parse(
            configuration["AuthSettings:RefreshTokenDays"] ?? "7");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task<(RefreshToken Entity, string PlainToken)> CreateRefreshTokenAsync(
        long modelId,
        string modelType,
        string? deviceInfo = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionName = null)
    {
        var plainToken = _jwtTokenService.GenerateRefreshToken();
        var tokenHash = _jwtTokenService.HashRefreshToken(plainToken);
        var familyId = _jwtTokenService.GenerateTokenFamilyId();

        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            ModelId = modelId,
            ModelType = modelType,
            TokenFamilyId = familyId,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            SessionName = sessionName,
            ExpiresAt = NowUtc.AddDays(_refreshTokenDays),
        };

        await _repository.AddAsync(refreshToken);
        return (refreshToken, plainToken);
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(refreshToken);
        var storedToken = await _repository.GetByTokenHashAsync(tokenHash);

        if (storedToken == null)
        {
            return RefreshTokenValidationResult.Invalid("Token not found");
        }

        if (storedToken.IsExpired)
        {
            return RefreshTokenValidationResult.Invalid("Token expired");
        }

        if (storedToken.IsRevoked)
        {

            var familyTokens = await _repository.GetByTokenFamilyAsync(storedToken.TokenFamilyId);


            if (familyTokens.Any(t => t.IsActive && t.Id != storedToken.Id))
            {

                await _repository.RevokeTokenFamilyAsync(storedToken.TokenFamilyId);
                return RefreshTokenValidationResult.TheftDetected(
                    "Token family revoked due to potential theft");
            }

            return RefreshTokenValidationResult.Invalid("Token revoked");
        }

        return RefreshTokenValidationResult.Valid(storedToken);
    }

    public async Task<(RefreshToken Entity, string PlainToken)> RotateRefreshTokenAsync(
        RefreshToken oldToken,
        string? deviceInfo = null,
        string? ipAddress = null,
        string? userAgent = null)
    {

        oldToken.RevokedAt = NowUtc;
        var newPlainToken = _jwtTokenService.GenerateRefreshToken();
        var newTokenHash = _jwtTokenService.HashRefreshToken(newPlainToken);

        var newRefreshToken = new RefreshToken
        {
            TokenHash = newTokenHash,
            ModelId = oldToken.ModelId,
            ModelType = oldToken.ModelType,
            TokenFamilyId = oldToken.TokenFamilyId,
            DeviceInfo = deviceInfo ?? oldToken.DeviceInfo,
            IpAddress = ipAddress ?? oldToken.IpAddress,
            UserAgent = userAgent ?? oldToken.UserAgent,
            SessionName = oldToken.SessionName,
            ExpiresAt = NowUtc.AddDays(_refreshTokenDays),
        };


        oldToken.ReplacedByTokenHash = newTokenHash;
        await _repository.UpdateAsync(oldToken);
        await _repository.AddAsync(newRefreshToken);

        return (newRefreshToken, newPlainToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(refreshToken);
        await _repository.RevokeTokenAsync(tokenHash);
    }

    public async Task RevokeModelTokensAsync(long modelId, string modelType)
    {
        await _repository.RevokeModelTokensAsync(modelId, modelType);
    }
    public async Task RevokeModelTokensBulkAsync(List<long> modelIds, string modelType)
    {
        await _repository.RevokeModelTokensBulkAsync(modelIds, modelType);
    }
    public async Task RevokeTokenFamilyAsync(string tokenFamilyId)
    {
        await _repository.RevokeTokenFamilyAsync(tokenFamilyId);
    }

    public async Task UpdateLastUsedAsync(long sessionId)
    {
        await _repository.UpdateLastUsedAsync(sessionId);
    }

    public async Task HandleStatusChangeTokenRevocationAsync(List<long> adminIds, int status, int updatedCount, Domain.Common.Interfaces.IJwtTokenService jwtTokenService)
    {
        if (updatedCount > 0)
        {
            if (status == 0)
            {
                await RevokeModelTokensBulkAsync(adminIds, "Admin");
                foreach (var adminId in adminIds)
                {
                    await jwtTokenService.RevokeAllUserTokensAsync(adminId, "admin", expirationMinutes: 120);
                }
            }
            else if (status == 1)
            {
                await jwtTokenService.ClearUserRevocationBulkAsync(adminIds, "admin");
            }
        }
    }
}

