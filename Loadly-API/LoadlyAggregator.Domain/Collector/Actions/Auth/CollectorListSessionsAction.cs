using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorListSessionsAction : IAction<CollectorListSessionsDto, object>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public CollectorListSessionsAction(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }
    public async Task<object> ExecuteAsync(CollectorListSessionsDto dto)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsByModelAsync(
            dto.UserId,
            "Collector"
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
