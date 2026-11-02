using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorRevokeSessionAction : IAction<CollectorRevokeSessionDto, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly TimeProvider _timeProvider;
    public CollectorRevokeSessionAction(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        TimeProvider timeProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _timeProvider = timeProvider;
    }
    public async Task<bool> ExecuteAsync(CollectorRevokeSessionDto dto)
    {
        var session = await _refreshTokenRepository.GetByIdAsync(dto.SessionId)
            ?? throw new NotFoundException("sessions.not_found");
        _ = session.ModelId == dto.UserId && session.ModelType == "Collector"
            ? true
            : throw new ForbiddenException("sessions.permission_denied_revoke");
        await (session.IsRevoked
            ? Task.CompletedTask
            : UpdateSessionAsRevokedAsync(session));
        await Task.WhenAll(
            _refreshTokenRepository.RevokeTokenFamilyAsync(session.TokenFamilyId),
            _jwtTokenService.RevokeTokenFamilyAsync(session.TokenFamilyId, expirationMinutes: 120));
        return true;
    }

    private async Task UpdateSessionAsRevokedAsync(Domain.Common.Entities.RefreshToken session)
    {
        session.RevokedAt = AppUtcTime.UtcNow(_timeProvider);
        await _refreshTokenRepository.UpdateAsync(session);
    }
}
