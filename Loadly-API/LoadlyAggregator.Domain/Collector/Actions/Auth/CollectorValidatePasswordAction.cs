using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorValidatePasswordAction : IVoidAction<CollectorValidatePasswordDto>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly AuthService _authService;

    public CollectorValidatePasswordAction(
        ICollectorRepository collectorRepository,
        AuthService authService)
    {
        _collectorRepository = collectorRepository;
        _authService = authService;
    }

    public async Task ExecuteAsync(CollectorValidatePasswordDto dto)
    {
        _authService.ValidateCredentials(
            await _collectorRepository.GetByIdAsync(dto.UserId),
            dto.Password);
    }
}



