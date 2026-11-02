using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;



public class CompanyRevokeSessionAction : IAction<CompanyRevokeSessionDto, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly TimeProvider _timeProvider;

    public CompanyRevokeSessionAction(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        TimeProvider timeProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _timeProvider = timeProvider;
    }

    public async Task<bool> ExecuteAsync(CompanyRevokeSessionDto dto)
    {
        var session = await _refreshTokenRepository.GetByIdAsync(dto.SessionId)
            ?? throw new NotFoundException("sessions.not_found");
        _ = session.ModelId == dto.UserId && session.ModelType == "Company"
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
