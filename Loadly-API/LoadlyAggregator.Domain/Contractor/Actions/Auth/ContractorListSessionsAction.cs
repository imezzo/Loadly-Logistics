using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorListSessionsAction : IAction<ContractorListSessionsDto, object>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ContractorListSessionsAction(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<object> ExecuteAsync(ContractorListSessionsDto dto)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsByModelAsync(
            dto.UserId,
            "contractor"
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

