using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;




public class DriverListSessionsAction : IAction<DriverListSessionsDto, object>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public DriverListSessionsAction(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<object> ExecuteAsync(DriverListSessionsDto dto)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsByModelAsync(
            dto.UserId,
            "Driver"
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

