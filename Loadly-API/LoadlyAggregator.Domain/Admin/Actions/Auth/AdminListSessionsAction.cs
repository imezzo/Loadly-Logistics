using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminListSessionsAction : IAction<AdminListSessionsDto, object>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public AdminListSessionsAction(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }
    public async Task<object> ExecuteAsync(AdminListSessionsDto dto)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsByModelAsync(
            dto.UserId,
            "Admin"
        );
        return new
        {
            Sessions = sessions.Select(rt => new
            {
                TokenFamilyId = rt.TokenFamilyId,
                DeviceInfo = rt.DeviceInfo,
                IpAddress = rt.IpAddress,
                UserAgent = rt.UserAgent,
                CreatedAt = rt.CreatedAt,
                ExpiresAt = rt.ExpiresAt,
                IsCurrent = rt.TokenFamilyId == dto.CurrentTokenFamilyId
            }).ToList()
        };
    }
}
