using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;



public class CollectorGetProfileAction : IAction<long, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPermissionService _permissionService;
    public CollectorGetProfileAction(ICollectorRepository collectorRepository, IPermissionService permissionService)
    {
        _collectorRepository = collectorRepository;
        _permissionService = permissionService;
    }
    public async Task<object> ExecuteAsync(long userId)
    {
        var collector = await _collectorRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("collectors.not_found");
        return new
        {
            Id = collector.Id,
            FullName = collector.FullName,
            UserName = collector.UserName,
            Email = collector.Email,
            Phone = collector.Phone,
            UserType = "collector",
            MfaChannel = (int)collector.MfaChannel,
            EmailVerified = collector.EmailVerified,
            PhoneVerified = collector.PhoneVerified,
            Status = (int)collector.Status,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector),
            CreatedAt = collector.CreatedAt,
            UpdatedAt = collector.UpdatedAt
        };
    }
}
