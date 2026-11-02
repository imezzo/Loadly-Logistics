using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanyListSessionsAction : IAction<CompanyListSessionsDto, object>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public CompanyListSessionsAction(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<object> ExecuteAsync(CompanyListSessionsDto dto)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsByModelAsync(
            dto.UserId,
            "Company"
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

